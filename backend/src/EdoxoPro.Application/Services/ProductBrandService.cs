using AutoMapper;
using EdoxoPro.Application.Common;
using EdoxoPro.Application.DTOs.Products;
using EdoxoPro.Application.Interfaces;
using EdoxoPro.Domain.Entities;
using EdoxoPro.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace EdoxoPro.Application.Services;

public class ProductBrandService : IProductBrandService
{
    private readonly IGenericRepository<ProductBrand> _repo;
    private readonly IMapper _mapper;
    private readonly ILogger<ProductBrandService> _logger;

    public ProductBrandService(IGenericRepository<ProductBrand> repo, IMapper mapper, ILogger<ProductBrandService> logger)
    {
        _repo = repo; _mapper = mapper; _logger = logger;
    }

    public async Task<ApiResponse<IEnumerable<ProductBrandDto>>> GetAllAsync()
    {
        try
        {
            var entities = await _repo.GetAllAsync();
            var dtos = _mapper.Map<List<ProductBrandDto>>(entities.Where(e => !e.IsDeleted));
            return ApiResponse<IEnumerable<ProductBrandDto>>.Ok(dtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في جلب ماركات المنتجات");
            return ApiResponse<IEnumerable<ProductBrandDto>>.Fail("حدث خطأ أثناء جلب الماركات");
        }
    }

    public async Task<ApiResponse<ProductBrandDto>> GetByIdAsync(int id)
    {
        try
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null || entity.IsDeleted) return ApiResponse<ProductBrandDto>.Fail("الماركة غير موجودة");
            return ApiResponse<ProductBrandDto>.Ok(_mapper.Map<ProductBrandDto>(entity));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في جلب الماركة {Id}", id);
            return ApiResponse<ProductBrandDto>.Fail("حدث خطأ أثناء جلب الماركة");
        }
    }

    public async Task<ApiResponse<ProductBrandDto>> CreateAsync(CreateProductBrandDto request)
    {
        try
        {
            var entity = _mapper.Map<ProductBrand>(request);
            entity.CreatedAt = DateTime.UtcNow; entity.IsActive = true;
            await _repo.AddAsync(entity); await _repo.SaveChangesAsync();
            return ApiResponse<ProductBrandDto>.Ok(_mapper.Map<ProductBrandDto>(entity), "تم إنشاء الماركة بنجاح");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في إنشاء ماركة جديدة");
            return ApiResponse<ProductBrandDto>.Fail("حدث خطأ أثناء إنشاء الماركة");
        }
    }

    public async Task<ApiResponse<ProductBrandDto>> UpdateAsync(int id, UpdateProductBrandDto request)
    {
        try
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null || entity.IsDeleted) return ApiResponse<ProductBrandDto>.Fail("الماركة غير موجودة");
            _mapper.Map(request, entity);
            entity.UpdatedAt = DateTime.UtcNow;
            _repo.Update(entity); await _repo.SaveChangesAsync();
            return ApiResponse<ProductBrandDto>.Ok(_mapper.Map<ProductBrandDto>(entity), "تم تحديث الماركة بنجاح");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في تحديث الماركة {Id}", id);
            return ApiResponse<ProductBrandDto>.Fail("حدث خطأ أثناء تحديث الماركة");
        }
    }

    public async Task<ApiResponse<string>> DeleteAsync(int id)
    {
        try
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null || entity.IsDeleted) return ApiResponse<string>.Fail("الماركة غير موجودة");
            _repo.SoftDelete(entity); await _repo.SaveChangesAsync();
            return ApiResponse<string>.Ok(string.Empty, "تم حذف الماركة بنجاح");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في حذف الماركة {Id}", id);
            return ApiResponse<string>.Fail("حدث خطأ أثناء حذف الماركة");
        }
    }
}
