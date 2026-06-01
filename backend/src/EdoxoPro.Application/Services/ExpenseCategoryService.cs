using AutoMapper;
using EdoxoPro.Application.Common;
using EdoxoPro.Application.DTOs.Expenses;
using EdoxoPro.Application.Interfaces;
using EdoxoPro.Domain.Entities;
using EdoxoPro.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace EdoxoPro.Application.Services;

public class ExpenseCategoryService : IExpenseCategoryService
{
    private readonly IGenericRepository<ExpenseCategory> _repo;
    private readonly IMapper _mapper;
    private readonly ILogger<ExpenseCategoryService> _logger;

    public ExpenseCategoryService(
        IGenericRepository<ExpenseCategory> repo,
        IMapper mapper,
        ILogger<ExpenseCategoryService> logger)
    {
        _repo = repo;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ApiResponse<IEnumerable<ExpenseCategoryDto>>> GetAllAsync()
    {
        try
        {
            var entities = await _repo.GetAllAsync();
            var dtos = _mapper.Map<List<ExpenseCategoryDto>>(entities.Where(e => !e.IsDeleted));
            return ApiResponse<IEnumerable<ExpenseCategoryDto>>.Ok(dtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في جلب تصنيفات المصروفات");
            return ApiResponse<IEnumerable<ExpenseCategoryDto>>.Fail("حدث خطأ أثناء جلب التصنيفات");
        }
    }

    public async Task<ApiResponse<ExpenseCategoryDto>> GetByIdAsync(int id)
    {
        try
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null || entity.IsDeleted)
                return ApiResponse<ExpenseCategoryDto>.Fail("التصنيف غير موجود");

            var dto = _mapper.Map<ExpenseCategoryDto>(entity);
            return ApiResponse<ExpenseCategoryDto>.Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في جلب تصنيف المصروفات {Id}", id);
            return ApiResponse<ExpenseCategoryDto>.Fail("حدث خطأ أثناء جلب التصنيف");
        }
    }

    public async Task<ApiResponse<ExpenseCategoryDto>> CreateAsync(CreateExpenseCategoryDto request)
    {
        try
        {
            var entity = _mapper.Map<ExpenseCategory>(request);
            entity.CreatedAt = DateTime.UtcNow;
            entity.IsActive = true;

            await _repo.AddAsync(entity);
            await _repo.SaveChangesAsync();

            var dto = _mapper.Map<ExpenseCategoryDto>(entity);
            return ApiResponse<ExpenseCategoryDto>.Ok(dto, "تم إنشاء التصنيف بنجاح");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في إنشاء تصنيف مصروفات جديد");
            return ApiResponse<ExpenseCategoryDto>.Fail("حدث خطأ أثناء إنشاء التصنيف");
        }
    }

    public async Task<ApiResponse<ExpenseCategoryDto>> UpdateAsync(int id, UpdateExpenseCategoryDto request)
    {
        try
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null || entity.IsDeleted)
                return ApiResponse<ExpenseCategoryDto>.Fail("التصنيف غير موجود");

            _mapper.Map(request, entity);
            entity.UpdatedAt = DateTime.UtcNow;
            _repo.Update(entity);
            await _repo.SaveChangesAsync();

            var dto = _mapper.Map<ExpenseCategoryDto>(entity);
            return ApiResponse<ExpenseCategoryDto>.Ok(dto, "تم تحديث التصنيف بنجاح");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في تحديث تصنيف المصروفات {Id}", id);
            return ApiResponse<ExpenseCategoryDto>.Fail("حدث خطأ أثناء تحديث التصنيف");
        }
    }

    public async Task<ApiResponse<string>> DeleteAsync(int id)
    {
        try
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null || entity.IsDeleted)
                return ApiResponse<string>.Fail("التصنيف غير موجود");

            _repo.SoftDelete(entity);
            await _repo.SaveChangesAsync();

            return ApiResponse<string>.Ok(string.Empty, "تم حذف التصنيف بنجاح");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في حذف تصنيف المصروفات {Id}", id);
            return ApiResponse<string>.Fail("حدث خطأ أثناء حذف التصنيف");
        }
    }
}
