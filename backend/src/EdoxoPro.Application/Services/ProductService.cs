using AutoMapper;
using EdoxoPro.Application.Common;
using EdoxoPro.Application.DTOs.Products;
using EdoxoPro.Application.Interfaces;
using EdoxoPro.Domain.Entities;
using EdoxoPro.Domain.Enums;
using EdoxoPro.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace EdoxoPro.Application.Services;

public class ProductService : IProductService
{
    private readonly IGenericRepository<Product> _productRepo;
    private readonly IGenericRepository<StockMovement> _stockMovementRepo;
    private readonly IMapper _mapper;
    private readonly ILogger<ProductService> _logger;

    public ProductService(IGenericRepository<Product> productRepo, IGenericRepository<StockMovement> stockMovementRepo, IMapper mapper, ILogger<ProductService> logger)
    {
        _productRepo = productRepo;
        _stockMovementRepo = stockMovementRepo;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ApiResponse<PagedResult<ProductDto>>> GetAllAsync(ProductFilterRequest request)
    {
        try
        {
            var items = await _productRepo.FindAsync(p => !p.IsDeleted);
            var query = items.AsQueryable();
            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var s = request.Search.ToLower();
                query = query.Where(p => p.Name.ToLower().Contains(s) || p.SKU.ToLower().Contains(s) || (p.Barcode != null && p.Barcode.ToLower().Contains(s)));
            }
            if (request.CategoryId.HasValue) query = query.Where(p => p.CategoryId == request.CategoryId);
            if (request.BrandId.HasValue) query = query.Where(p => p.BrandId == request.BrandId);
            if (request.IsActive.HasValue) query = query.Where(p => p.IsActive == request.IsActive);
            if (request.LowStock == true) query = query.Where(p => p.CurrentStock <= p.MinStock);
            var total = query.Count();
            var list = query.OrderByDescending(p => p.Id).Skip((request.Page - 1) * request.PageSize).Take(request.PageSize).ToList();
            var dtos = _mapper.Map<List<ProductDto>>(list);
            var result = new PagedResult<ProductDto> { Items = dtos, TotalCount = total, Page = request.Page, PageSize = request.PageSize };
            return ApiResponse<PagedResult<ProductDto>>.Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في جلب قائمة المنتجات");
            return ApiResponse<PagedResult<ProductDto>>.Fail("حدث خطأ أثناء جلب المنتجات");
        }
    }

    public async Task<ApiResponse<ProductDto>> GetByIdAsync(int id)
    {
        try
        {
            var entity = await _productRepo.GetByIdAsync(id);
            if (entity == null || entity.IsDeleted) return ApiResponse<ProductDto>.Fail("المنتج غير موجود");
            return ApiResponse<ProductDto>.Ok(_mapper.Map<ProductDto>(entity));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في جلب المنتج {Id}", id);
            return ApiResponse<ProductDto>.Fail("حدث خطأ أثناء جلب المنتج");
        }
    }

    public async Task<ApiResponse<ProductDto>> CreateAsync(CreateProductDto request)
    {
        try
        {
            var entity = _mapper.Map<Product>(request);
            entity.CreatedAt = DateTime.UtcNow;
            entity.IsActive = true;
            await _productRepo.AddAsync(entity);
            await _productRepo.SaveChangesAsync();
            return ApiResponse<ProductDto>.Ok(_mapper.Map<ProductDto>(entity), "تم إنشاء المنتج بنجاح");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في إنشاء منتج جديد");
            return ApiResponse<ProductDto>.Fail("حدث خطأ أثناء إنشاء المنتج");
        }
    }

    public async Task<ApiResponse<ProductDto>> UpdateAsync(int id, UpdateProductDto request)
    {
        try
        {
            var entity = await _productRepo.GetByIdAsync(id);
            if (entity == null || entity.IsDeleted) return ApiResponse<ProductDto>.Fail("المنتج غير موجود");
            _mapper.Map(request, entity);
            entity.UpdatedAt = DateTime.UtcNow;
            _productRepo.Update(entity);
            await _productRepo.SaveChangesAsync();
            return ApiResponse<ProductDto>.Ok(_mapper.Map<ProductDto>(entity), "تم تحديث المنتج بنجاح");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في تحديث المنتج {Id}", id);
            return ApiResponse<ProductDto>.Fail("حدث خطأ أثناء تحديث المنتج");
        }
    }

    public async Task<ApiResponse<string>> DeleteAsync(int id)
    {
        try
        {
            var entity = await _productRepo.GetByIdAsync(id);
            if (entity == null || entity.IsDeleted) return ApiResponse<string>.Fail("المنتج غير موجود");
            _productRepo.SoftDelete(entity);
            await _productRepo.SaveChangesAsync();
            return ApiResponse<string>.Ok(string.Empty, "تم حذف المنتج بنجاح");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في حذف المنتج {Id}", id);
            return ApiResponse<string>.Fail("حدث خطأ أثناء حذف المنتج");
        }
    }

    public async Task<ApiResponse<ProductDto>> GetByBarcodeAsync(string barcode)
    {
        try
        {
            var entity = (await _productRepo.FindAsync(p => p.Barcode == barcode && !p.IsDeleted)).FirstOrDefault();
            if (entity == null) return ApiResponse<ProductDto>.Fail("المنتج غير موجود لهذا الباركود");
            return ApiResponse<ProductDto>.Ok(_mapper.Map<ProductDto>(entity));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في جلب المنتج بالباركود {Barcode}", barcode);
            return ApiResponse<ProductDto>.Fail("حدث خطأ أثناء جلب المنتج بالباركود");
        }
    }

    public async Task<ApiResponse<string>> UpdateStockAsync(int id, double quantity)
    {
        try
        {
            var entity = await _productRepo.GetByIdAsync(id);
            if (entity == null || entity.IsDeleted) return ApiResponse<string>.Fail("المنتج غير موجود");
            entity.CurrentStock += quantity;
            entity.UpdatedAt = DateTime.UtcNow;
            _productRepo.Update(entity);
            await _stockMovementRepo.AddAsync(new StockMovement
            {
                ProductId = id, WarehouseId = 1, Quantity = quantity,
                Type = quantity > 0 ? StockMovementType.In : StockMovementType.Out,
                Date = DateTime.UtcNow, Notes = quantity > 0 ? "إضافة مخزون يدوي" : "تخفيض مخزون يدوي"
            });
            await _productRepo.SaveChangesAsync();
            return ApiResponse<string>.Ok(string.Empty, "تم تحديث المخزون بنجاح");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في تحديث مخزون المنتج {Id}", id);
            return ApiResponse<string>.Fail("حدث خطأ أثناء تحديث المخزون");
        }
    }
}
