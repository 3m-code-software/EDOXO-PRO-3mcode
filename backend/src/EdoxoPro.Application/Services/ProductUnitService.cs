using AutoMapper;
using EdoxoPro.Application.Common;
using EdoxoPro.Application.DTOs.Products;
using EdoxoPro.Application.Interfaces;
using EdoxoPro.Domain.Entities;
using EdoxoPro.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace EdoxoPro.Application.Services;

public class ProductUnitService : IProductUnitService
{
    private readonly IGenericRepository<ProductUnit> _repo;
    private readonly IMapper _mapper;
    private readonly ILogger<ProductUnitService> _logger;

    public ProductUnitService(IGenericRepository<ProductUnit> repo, IMapper mapper, ILogger<ProductUnitService> logger)
    {
        _repo = repo; _mapper = mapper; _logger = logger;
    }

    public async Task<ApiResponse<IEnumerable<ProductUnitDto>>> GetAllAsync()
    {
        try
        {
            var entities = await _repo.GetAllAsync();
            var dtos = _mapper.Map<List<ProductUnitDto>>(entities.Where(e => !e.IsDeleted));
            return ApiResponse<IEnumerable<ProductUnitDto>>.Ok(dtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في جلب وحدات المنتجات");
            return ApiResponse<IEnumerable<ProductUnitDto>>.Fail("حدث خطأ أثناء جلب الوحدات");
        }
    }

    public async Task<ApiResponse<ProductUnitDto>> GetByIdAsync(int id)
    {
        try
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null || entity.IsDeleted) return ApiResponse<ProductUnitDto>.Fail("الوحدة غير موجودة");
            return ApiResponse<ProductUnitDto>.Ok(_mapper.Map<ProductUnitDto>(entity));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في جلب الوحدة {Id}", id);
            return ApiResponse<ProductUnitDto>.Fail("حدث خطأ أثناء جلب الوحدة");
        }
    }

    public async Task<ApiResponse<ProductUnitDto>> CreateAsync(CreateProductUnitDto request)
    {
        try
        {
            var entity = _mapper.Map<ProductUnit>(request);
            entity.CreatedAt = DateTime.UtcNow; entity.IsActive = true;
            await _repo.AddAsync(entity); await _repo.SaveChangesAsync();
            return ApiResponse<ProductUnitDto>.Ok(_mapper.Map<ProductUnitDto>(entity), "تم إنشاء الوحدة بنجاح");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في إنشاء وحدة جديدة");
            return ApiResponse<ProductUnitDto>.Fail("حدث خطأ أثناء إنشاء الوحدة");
        }
    }

    public async Task<ApiResponse<ProductUnitDto>> UpdateAsync(int id, UpdateProductUnitDto request)
    {
        try
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null || entity.IsDeleted) return ApiResponse<ProductUnitDto>.Fail("الوحدة غير موجودة");
            _mapper.Map(request, entity);
            entity.UpdatedAt = DateTime.UtcNow;
            _repo.Update(entity); await _repo.SaveChangesAsync();
            return ApiResponse<ProductUnitDto>.Ok(_mapper.Map<ProductUnitDto>(entity), "تم تحديث الوحدة بنجاح");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في تحديث الوحدة {Id}", id);
            return ApiResponse<ProductUnitDto>.Fail("حدث خطأ أثناء تحديث الوحدة");
        }
    }

    public async Task<ApiResponse<string>> DeleteAsync(int id)
    {
        try
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null || entity.IsDeleted) return ApiResponse<string>.Fail("الوحدة غير موجودة");
            _repo.SoftDelete(entity); await _repo.SaveChangesAsync();
            return ApiResponse<string>.Ok(string.Empty, "تم حذف الوحدة بنجاح");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في حذف الوحدة {Id}", id);
            return ApiResponse<string>.Fail("حدث خطأ أثناء حذف الوحدة");
        }
    }
}
