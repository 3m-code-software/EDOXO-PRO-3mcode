using AutoMapper;
using EdoxoPro.Application.Common;
using EdoxoPro.Application.DTOs.Sales;
using EdoxoPro.Application.Interfaces;
using EdoxoPro.Domain.Entities;
using EdoxoPro.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace EdoxoPro.Application.Services;

public class SaleReturnService : ISaleReturnService
{
    private readonly IGenericRepository<SaleReturn> _returnRepo;
    private readonly IGenericRepository<SaleReturnItem> _returnItemRepo;
    private readonly IGenericRepository<SaleItem> _saleItemRepo;
    private readonly IGenericRepository<Product> _productRepo;
    private readonly IMapper _mapper;
    private readonly ILogger<SaleReturnService> _logger;

    public SaleReturnService(IGenericRepository<SaleReturn> returnRepo, IGenericRepository<SaleReturnItem> returnItemRepo, IGenericRepository<SaleItem> saleItemRepo, IGenericRepository<Product> productRepo, IMapper mapper, ILogger<SaleReturnService> logger)
    {
        _returnRepo = returnRepo; _returnItemRepo = returnItemRepo; _saleItemRepo = saleItemRepo; _productRepo = productRepo; _mapper = mapper; _logger = logger;
    }

    public async Task<ApiResponse<PagedResult<SaleReturnDto>>> GetAllAsync(FilterRequest request)
    {
        try
        {
            var items = await _returnRepo.FindAsync(r => !r.IsDeleted);
            var query = items.AsQueryable();
            if (!string.IsNullOrWhiteSpace(request.Search))
                query = query.Where(r => r.ReturnNumber.ToLower().Contains(request.Search.ToLower()));
            var total = query.Count();
            var list = query.OrderByDescending(r => r.Date).Skip((request.Page - 1) * request.PageSize).Take(request.PageSize).ToList();
            var dtos = _mapper.Map<List<SaleReturnDto>>(list);
            return ApiResponse<PagedResult<SaleReturnDto>>.Ok(new PagedResult<SaleReturnDto> { Items = dtos, TotalCount = total, Page = request.Page, PageSize = request.PageSize });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في جلب مرتجعات المبيعات");
            return ApiResponse<PagedResult<SaleReturnDto>>.Fail("حدث خطأ أثناء جلب مرتجعات المبيعات");
        }
    }

    public async Task<ApiResponse<SaleReturnDto>> GetByIdAsync(int id)
    {
        try
        {
            var entity = await _returnRepo.GetByIdAsync(id);
            if (entity == null || entity.IsDeleted) return ApiResponse<SaleReturnDto>.Fail("مرتجع البيع غير موجود");
            return ApiResponse<SaleReturnDto>.Ok(_mapper.Map<SaleReturnDto>(entity));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في جلب مرتجع البيع {Id}", id);
            return ApiResponse<SaleReturnDto>.Fail("حدث خطأ أثناء جلب مرتجع البيع");
        }
    }

    public async Task<ApiResponse<SaleReturnDto>> CreateAsync(CreateSaleReturnDto request)
    {
        try
        {
            var count = await _returnRepo.CountAsync();
            var returnNumber = $"SRTN-{count + 1:D5}";
            var saleReturn = new SaleReturn { ReturnNumber = returnNumber, SaleId = request.SaleId, Date = DateTime.UtcNow, Reason = request.Reason, CreatedAt = DateTime.UtcNow };
            await _returnRepo.AddAsync(saleReturn);
            await _returnRepo.SaveChangesAsync();

            decimal total = 0;
            foreach (var itemReq in request.Items)
            {
                var itemTotal = (decimal)(itemReq.Quantity * (double)itemReq.UnitPrice);
                total += itemTotal;
                await _returnItemRepo.AddAsync(new SaleReturnItem { ReturnId = saleReturn.Id, SaleItemId = itemReq.SaleItemId, Quantity = itemReq.Quantity, UnitPrice = itemReq.UnitPrice, Total = itemTotal, CreatedAt = DateTime.UtcNow });

                var saleItem = await _saleItemRepo.GetByIdAsync(itemReq.SaleItemId);
                if (saleItem != null)
                {
                    var product = await _productRepo.GetByIdAsync(saleItem.ProductId);
                    if (product != null)
                    {
                        product.CurrentStock += itemReq.Quantity;
                        product.UpdatedAt = DateTime.UtcNow;
                        _productRepo.Update(product);
                    }
                }
            }

            saleReturn.Total = total;
            _returnRepo.Update(saleReturn);
            await _returnRepo.SaveChangesAsync();
            return ApiResponse<SaleReturnDto>.Ok(_mapper.Map<SaleReturnDto>(saleReturn), "تم إنشاء مرتجع البيع بنجاح");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في إنشاء مرتجع بيع");
            return ApiResponse<SaleReturnDto>.Fail("حدث خطأ أثناء إنشاء مرتجع البيع");
        }
    }

    public async Task<ApiResponse<string>> DeleteAsync(int id)
    {
        try
        {
            var entity = await _returnRepo.GetByIdAsync(id);
            if (entity == null || entity.IsDeleted) return ApiResponse<string>.Fail("مرتجع البيع غير موجود");
            _returnRepo.SoftDelete(entity);
            await _returnRepo.SaveChangesAsync();
            return ApiResponse<string>.Ok(string.Empty, "تم حذف مرتجع البيع بنجاح");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في حذف مرتجع البيع {Id}", id);
            return ApiResponse<string>.Fail("حدث خطأ أثناء حذف مرتجع البيع");
        }
    }
}
