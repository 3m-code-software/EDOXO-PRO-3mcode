using AutoMapper;
using EdoxoPro.Application.Common;
using EdoxoPro.Application.DTOs.Purchases;
using EdoxoPro.Application.Interfaces;
using EdoxoPro.Domain.Entities;
using EdoxoPro.Domain.Enums;
using EdoxoPro.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace EdoxoPro.Application.Services;

public class PurchaseService : IPurchaseService
{
    private readonly IGenericRepository<Purchase> _purchaseRepo;
    private readonly IGenericRepository<PurchaseItem> _purchaseItemRepo;
    private readonly IGenericRepository<Product> _productRepo;
    private readonly IMapper _mapper;
    private readonly ILogger<PurchaseService> _logger;

    public PurchaseService(IGenericRepository<Purchase> purchaseRepo, IGenericRepository<PurchaseItem> purchaseItemRepo, IGenericRepository<Product> productRepo, IMapper mapper, ILogger<PurchaseService> logger)
    {
        _purchaseRepo = purchaseRepo; _purchaseItemRepo = purchaseItemRepo; _productRepo = productRepo; _mapper = mapper; _logger = logger;
    }

    public async Task<ApiResponse<PagedResult<PurchaseDto>>> GetAllAsync(PurchaseFilterRequest request)
    {
        try
        {
            var items = await _purchaseRepo.FindAsync(p => !p.IsDeleted);
            var query = items.AsQueryable();
            if (!string.IsNullOrWhiteSpace(request.Status)) query = query.Where(p => p.Status.ToString() == request.Status);
            if (request.SupplierId.HasValue) query = query.Where(p => p.SupplierId == request.SupplierId);
            if (request.DateFrom.HasValue) query = query.Where(p => p.Date >= request.DateFrom);
            if (request.DateTo.HasValue) query = query.Where(p => p.Date <= request.DateTo);
            var total = query.Count();
            var list = query.OrderByDescending(p => p.Date).Skip((request.Page - 1) * request.PageSize).Take(request.PageSize).ToList();
            var dtos = _mapper.Map<List<PurchaseDto>>(list);
            return ApiResponse<PagedResult<PurchaseDto>>.Ok(new PagedResult<PurchaseDto> { Items = dtos, TotalCount = total, Page = request.Page, PageSize = request.PageSize });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في جلب قائمة المشتريات");
            return ApiResponse<PagedResult<PurchaseDto>>.Fail("حدث خطأ أثناء جلب المشتريات");
        }
    }

    public async Task<ApiResponse<PurchaseDto>> GetByIdAsync(int id)
    {
        try
        {
            var entity = await _purchaseRepo.GetByIdAsync(id);
            if (entity == null || entity.IsDeleted) return ApiResponse<PurchaseDto>.Fail("أمر الشراء غير موجود");
            return ApiResponse<PurchaseDto>.Ok(_mapper.Map<PurchaseDto>(entity));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في جلب أمر الشراء {Id}", id);
            return ApiResponse<PurchaseDto>.Fail("حدث خطأ أثناء جلب أمر الشراء");
        }
    }

    public async Task<ApiResponse<PurchaseDto>> CreateAsync(CreatePurchaseDto request)
    {
        try
        {
            var count = await _purchaseRepo.CountAsync();
            var refNumber = $"PO-{count + 1:D5}";
            var purchase = new Purchase { ReferenceNumber = refNumber, SupplierId = request.SupplierId, BranchId = request.BranchId, Date = request.Date ?? DateTime.UtcNow, TaxRate = request.TaxRate, PaymentPeriod = request.PaymentPeriod, Notes = request.Notes, Status = PurchaseStatus.Draft, CreatedAt = DateTime.UtcNow };
            await _purchaseRepo.AddAsync(purchase);
            await _purchaseRepo.SaveChangesAsync();

            decimal subtotal = 0;
            foreach (var itemReq in request.Items)
            {
                var product = await _productRepo.GetByIdAsync(itemReq.ProductId);
                if (product == null || product.IsDeleted) return ApiResponse<PurchaseDto>.Fail($"المنتج {itemReq.ProductId} غير موجود");
                var itemTotal = (decimal)(itemReq.Quantity * (double)itemReq.UnitPrice);
                subtotal += itemTotal;
                await _purchaseItemRepo.AddAsync(new PurchaseItem { PurchaseId = purchase.Id, ProductId = itemReq.ProductId, VariantId = itemReq.VariantId, Quantity = itemReq.Quantity, UnitPrice = itemReq.UnitPrice, Total = itemTotal, CreatedAt = DateTime.UtcNow });

                product.CurrentStock += itemReq.Quantity;
                product.UpdatedAt = DateTime.UtcNow;
                _productRepo.Update(product);
            }

            purchase.Subtotal = subtotal;
            purchase.Tax = subtotal * purchase.TaxRate / 100;
            purchase.Total = subtotal + purchase.Tax;
            _purchaseRepo.Update(purchase);
            await _purchaseRepo.SaveChangesAsync();
            return ApiResponse<PurchaseDto>.Ok(_mapper.Map<PurchaseDto>(purchase), "تم إنشاء أمر الشراء بنجاح");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في إنشاء أمر شراء");
            return ApiResponse<PurchaseDto>.Fail("حدث خطأ أثناء إنشاء أمر الشراء");
        }
    }

    public async Task<ApiResponse<PurchaseDto>> UpdateAsync(int id, UpdatePurchaseDto request)
    {
        try
        {
            var entity = await _purchaseRepo.GetByIdAsync(id);
            if (entity == null || entity.IsDeleted) return ApiResponse<PurchaseDto>.Fail("أمر الشراء غير موجود");
            if (!string.IsNullOrWhiteSpace(request.Status)) entity.Status = Enum.Parse<PurchaseStatus>(request.Status);
            if (request.Notes != null) entity.Notes = request.Notes;
            entity.UpdatedAt = DateTime.UtcNow;
            _purchaseRepo.Update(entity);
            await _purchaseRepo.SaveChangesAsync();
            return ApiResponse<PurchaseDto>.Ok(_mapper.Map<PurchaseDto>(entity), "تم تحديث أمر الشراء بنجاح");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في تحديث أمر الشراء {Id}", id);
            return ApiResponse<PurchaseDto>.Fail("حدث خطأ أثناء تحديث أمر الشراء");
        }
    }

    public async Task<ApiResponse<string>> DeleteAsync(int id)
    {
        try
        {
            var entity = await _purchaseRepo.GetByIdAsync(id);
            if (entity == null || entity.IsDeleted) return ApiResponse<string>.Fail("أمر الشراء غير موجود");
            if (entity.Status == PurchaseStatus.Received) return ApiResponse<string>.Fail("لا يمكن حذف أمر شراء مستلم");
            _purchaseRepo.SoftDelete(entity);
            await _purchaseRepo.SaveChangesAsync();
            return ApiResponse<string>.Ok(string.Empty, "تم حذف أمر الشراء بنجاح");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في حذف أمر الشراء {Id}", id);
            return ApiResponse<string>.Fail("حدث خطأ أثناء حذف أمر الشراء");
        }
    }

    public async Task<ApiResponse<string>> ReceiveAsync(int id)
    {
        try
        {
            var entity = await _purchaseRepo.GetByIdAsync(id);
            if (entity == null || entity.IsDeleted) return ApiResponse<string>.Fail("أمر الشراء غير موجود");
            if (entity.Status != PurchaseStatus.Draft && entity.Status != PurchaseStatus.Ordered) return ApiResponse<string>.Fail("لا يمكن استلام أمر الشراء في حالته الحالية");
            entity.Status = PurchaseStatus.Received;
            entity.UpdatedAt = DateTime.UtcNow;
            _purchaseRepo.Update(entity);
            await _purchaseRepo.SaveChangesAsync();
            return ApiResponse<string>.Ok(string.Empty, "تم استلام أمر الشراء بنجاح");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في استلام أمر الشراء {Id}", id);
            return ApiResponse<string>.Fail("حدث خطأ أثناء استلام أمر الشراء");
        }
    }
}
