using AutoMapper;
using EdoxoPro.Application.Common;
using EdoxoPro.Application.DTOs.Products;
using EdoxoPro.Application.Interfaces;
using EdoxoPro.Domain.Entities;
using EdoxoPro.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace EdoxoPro.Application.Services;

public class ProductCategoryService : IProductCategoryService
{
    private readonly IGenericRepository<ProductCategory> _repo;
    private readonly IMapper _mapper;
    private readonly ILogger<ProductCategoryService> _logger;

    public ProductCategoryService(IGenericRepository<ProductCategory> repo, IMapper mapper, ILogger<ProductCategoryService> logger)
    {
        _repo = repo; _mapper = mapper; _logger = logger;
    }

    public async Task<ApiResponse<IEnumerable<ProductCategoryDto>>> GetAllAsync()
    {
        try
        {
            var entities = await _repo.GetAllAsync();
            var dtos = _mapper.Map<List<ProductCategoryDto>>(entities.Where(e => !e.IsDeleted));
            return ApiResponse<IEnumerable<ProductCategoryDto>>.Ok(dtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في جلب تصنيفات المنتجات");
            return ApiResponse<IEnumerable<ProductCategoryDto>>.Fail("حدث خطأ أثناء جلب التصنيفات");
        }
    }

    public async Task<ApiResponse<ProductCategoryDto>> GetByIdAsync(int id)
    {
        try
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null || entity.IsDeleted) return ApiResponse<ProductCategoryDto>.Fail("التصنيف غير موجود");
            return ApiResponse<ProductCategoryDto>.Ok(_mapper.Map<ProductCategoryDto>(entity));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في جلب التصنيف {Id}", id);
            return ApiResponse<ProductCategoryDto>.Fail("حدث خطأ أثناء جلب التصنيف");
        }
    }

    public async Task<ApiResponse<ProductCategoryDto>> CreateAsync(CreateProductCategoryDto request)
    {
        try
        {
            var entity = _mapper.Map<ProductCategory>(request);
            entity.CreatedAt = DateTime.UtcNow; entity.IsActive = true;
            await _repo.AddAsync(entity); await _repo.SaveChangesAsync();
            return ApiResponse<ProductCategoryDto>.Ok(_mapper.Map<ProductCategoryDto>(entity), "تم إنشاء التصنيف بنجاح");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في إنشاء تصنيف جديد");
            return ApiResponse<ProductCategoryDto>.Fail("حدث خطأ أثناء إنشاء التصنيف");
        }
    }

    public async Task<ApiResponse<ProductCategoryDto>> UpdateAsync(int id, UpdateProductCategoryDto request)
    {
        try
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null || entity.IsDeleted) return ApiResponse<ProductCategoryDto>.Fail("التصنيف غير موجود");
            if (request.ParentId.HasValue && request.ParentId.Value == id)
                return ApiResponse<ProductCategoryDto>.Fail("لا يمكن تعيين التصنيف كأصل لنفسه");
            _mapper.Map(request, entity);
            entity.UpdatedAt = DateTime.UtcNow;
            _repo.Update(entity); await _repo.SaveChangesAsync();
            return ApiResponse<ProductCategoryDto>.Ok(_mapper.Map<ProductCategoryDto>(entity), "تم تحديث التصنيف بنجاح");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في تحديث التصنيف {Id}", id);
            return ApiResponse<ProductCategoryDto>.Fail("حدث خطأ أثناء تحديث التصنيف");
        }
    }

    public async Task<ApiResponse<string>> DeleteAsync(int id)
    {
        try
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null || entity.IsDeleted) return ApiResponse<string>.Fail("التصنيف غير موجود");
            _repo.SoftDelete(entity); await _repo.SaveChangesAsync();
            return ApiResponse<string>.Ok(string.Empty, "تم حذف التصنيف بنجاح");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في حذف التصنيف {Id}", id);
            return ApiResponse<string>.Fail("حدث خطأ أثناء حذف التصنيف");
        }
    }
}
