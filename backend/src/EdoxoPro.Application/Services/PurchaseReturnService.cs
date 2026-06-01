using AutoMapper;
using EdoxoPro.Application.Common;
using EdoxoPro.Application.DTOs.Purchases;
using EdoxoPro.Application.Interfaces;
using EdoxoPro.Domain.Entities;
using EdoxoPro.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace EdoxoPro.Application.Services;

public class PurchaseReturnService : IPurchaseReturnService
{
    private readonly IGenericRepository<PurchaseReturn> _returnRepo;
    private readonly IGenericRepository<PurchaseReturnItem> _returnItemRepo;
    private readonly IGenericRepository<PurchaseItem> _purchaseItemRepo;
    private readonly IGenericRepository<Product> _productRepo;
    private readonly IMapper _mapper;
    private readonly ILogger<PurchaseReturnService> _logger;

    public PurchaseReturnService(IGenericRepository<PurchaseReturn> returnRepo, IGenericRepository<PurchaseReturnItem> returnItemRepo, IGenericRepository<PurchaseItem> purchaseItemRepo, IGenericRepository<Product> productRepo, IMapper mapper, ILogger<PurchaseReturnService> logger)
    {
        _returnRepo = returnRepo; _returnItemRepo = returnItemRepo; _purchaseItemRepo = purchaseItemRepo; _productRepo = productRepo; _mapper = mapper; _logger = logger;
    }

    public async Task<ApiResponse<PagedResult<PurchaseReturnDto>>> GetAllAsync(FilterRequest request)
    {
        try
        {
            var items = await _returnRepo.FindAsync(r => !r.IsDeleted);
            var query = items.AsQueryable();
            if (!string.IsNullOrWhiteSpace(request.Search)) query = query.Where(r => r.ReturnNumber.ToLower().Contains(request.Search.ToLower()));
            var total = query.Count();
            var list = query.OrderByDescending(r => r.Date).Skip((request.Page - 1) * request.PageSize).Take(request.PageSize).ToList();
            var dtos = _mapper.Map<List<PurchaseReturnDto>>(list);
            return ApiResponse<PagedResult<PurchaseReturnDto>>.Ok(new PagedResult<PurchaseReturnDto> { Items = dtos, TotalCount = total, Page = request.Page, PageSize = request.PageSize });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في جلب مرتجعات المشتريات");
            return ApiResponse<PagedResult<PurchaseReturnDto>>.Fail("حدث خطأ أثناء جلب مرتجعات المشتريات");
        }
    }

    public async Task<ApiResponse<PurchaseReturnDto>> GetByIdAsync(int id)
    {
        try
        {
            var entity = await _returnRepo.GetByIdAsync(id);
            if (entity == null || entity.IsDeleted) return ApiResponse<PurchaseReturnDto>.Fail("مرتجع الشراء غير موجود");
            return ApiResponse<PurchaseReturnDto>.Ok(_mapper.Map<PurchaseReturnDto>(entity));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في جلب مرتجع الشراء {Id}", id);
            return ApiResponse<PurchaseReturnDto>.Fail("حدث خطأ أثناء جلب مرتجع الشراء");
        }
    }

    public async Task<ApiResponse<PurchaseReturnDto>> CreateAsync(CreatePurchaseReturnDto request)
    {
        try
        {
            var count = await _returnRepo.CountAsync();
            var returnNumber = $"PRTN-{count + 1:D5}";
            var purchaseReturn = new PurchaseReturn { ReturnNumber = returnNumber, PurchaseId = request.PurchaseId, Date = DateTime.UtcNow, Reason = request.Reason, CreatedAt = DateTime.UtcNow };
            await _returnRepo.AddAsync(purchaseReturn);
            await _returnRepo.SaveChangesAsync();

            decimal total = 0;
            foreach (var itemReq in request.Items)
            {
                var itemTotal = (decimal)(itemReq.Quantity * (double)itemReq.UnitPrice);
                total += itemTotal;
                await _returnItemRepo.AddAsync(new PurchaseReturnItem { ReturnId = purchaseReturn.Id, PurchaseItemId = itemReq.PurchaseItemId, Quantity = itemReq.Quantity, UnitPrice = itemReq.UnitPrice, CreatedAt = DateTime.UtcNow });

                var purchaseItem = await _purchaseItemRepo.GetByIdAsync(itemReq.PurchaseItemId);
                if (purchaseItem != null)
                {
                    var product = await _productRepo.GetByIdAsync(purchaseItem.ProductId);
                    if (product != null)
                    {
                        product.CurrentStock -= itemReq.Quantity;
                        product.UpdatedAt = DateTime.UtcNow;
                        _productRepo.Update(product);
                    }
                }
            }

            purchaseReturn.Total = total;
            _returnRepo.Update(purchaseReturn);
            await _returnRepo.SaveChangesAsync();
            return ApiResponse<PurchaseReturnDto>.Ok(_mapper.Map<PurchaseReturnDto>(purchaseReturn), "تم إنشاء مرتجع الشراء بنجاح");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في إنشاء مرتجع شراء");
            return ApiResponse<PurchaseReturnDto>.Fail("حدث خطأ أثناء إنشاء مرتجع الشراء");
        }
    }

    public async Task<ApiResponse<string>> DeleteAsync(int id)
    {
        try
        {
            var entity = await _returnRepo.GetByIdAsync(id);
            if (entity == null || entity.IsDeleted) return ApiResponse<string>.Fail("مرتجع الشراء غير موجود");
            _returnRepo.SoftDelete(entity);
            await _returnRepo.SaveChangesAsync();
            return ApiResponse<string>.Ok(string.Empty, "تم حذف مرتجع الشراء بنجاح");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في حذف مرتجع الشراء {Id}", id);
            return ApiResponse<string>.Fail("حدث خطأ أثناء حذف مرتجع الشراء");
        }
    }
}
