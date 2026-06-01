using AutoMapper;
using EdoxoPro.Application.Common;
using EdoxoPro.Application.DTOs.Expenses;
using EdoxoPro.Application.Interfaces;
using EdoxoPro.Domain.Entities;
using EdoxoPro.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace EdoxoPro.Application.Services;

public class ExpenseService : IExpenseService
{
    private readonly IGenericRepository<Expense> _expenseRepo;
    private readonly IMapper _mapper;
    private readonly ILogger<ExpenseService> _logger;

    public ExpenseService(
        IGenericRepository<Expense> expenseRepo,
        IMapper mapper,
        ILogger<ExpenseService> logger)
    {
        _expenseRepo = expenseRepo;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ApiResponse<PagedResult<ExpenseDto>>> GetAllAsync(FilterRequest request)
    {
        try
        {
            var items = await _expenseRepo.FindAsync(e => !e.IsDeleted);
            var query = items.AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var s = request.Search.ToLower();
                query = query.Where(e => e.Description != null && e.Description.ToLower().Contains(s));
            }

            var total = query.Count();
            var list = query.OrderByDescending(e => e.Date)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            var dtos = _mapper.Map<List<ExpenseDto>>(list);
            var result = new PagedResult<ExpenseDto>
            {
                Items = dtos,
                TotalCount = total,
                Page = request.Page,
                PageSize = request.PageSize
            };

            return ApiResponse<PagedResult<ExpenseDto>>.Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في جلب المصروفات");
            return ApiResponse<PagedResult<ExpenseDto>>.Fail("حدث خطأ أثناء جلب المصروفات");
        }
    }

    public async Task<ApiResponse<ExpenseDto>> GetByIdAsync(int id)
    {
        try
        {
            var entity = await _expenseRepo.GetByIdAsync(id);
            if (entity == null || entity.IsDeleted)
                return ApiResponse<ExpenseDto>.Fail("المصروف غير موجود");

            var dto = _mapper.Map<ExpenseDto>(entity);
            return ApiResponse<ExpenseDto>.Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في جلب المصروف {Id}", id);
            return ApiResponse<ExpenseDto>.Fail("حدث خطأ أثناء جلب المصروف");
        }
    }

    public async Task<ApiResponse<ExpenseDto>> CreateAsync(CreateExpenseDto request)
    {
        try
        {
            var entity = _mapper.Map<Expense>(request);
            entity.CreatedAt = DateTime.UtcNow;

            await _expenseRepo.AddAsync(entity);
            await _expenseRepo.SaveChangesAsync();

            var dto = _mapper.Map<ExpenseDto>(entity);
            return ApiResponse<ExpenseDto>.Ok(dto, "تم إنشاء المصروف بنجاح");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في إنشاء مصروف جديد");
            return ApiResponse<ExpenseDto>.Fail("حدث خطأ أثناء إنشاء المصروف");
        }
    }

    public async Task<ApiResponse<ExpenseDto>> UpdateAsync(int id, UpdateExpenseDto request)
    {
        try
        {
            var entity = await _expenseRepo.GetByIdAsync(id);
            if (entity == null || entity.IsDeleted)
                return ApiResponse<ExpenseDto>.Fail("المصروف غير موجود");

            _mapper.Map(request, entity);
            entity.UpdatedAt = DateTime.UtcNow;
            _expenseRepo.Update(entity);
            await _expenseRepo.SaveChangesAsync();

            var dto = _mapper.Map<ExpenseDto>(entity);
            return ApiResponse<ExpenseDto>.Ok(dto, "تم تحديث المصروف بنجاح");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في تحديث المصروف {Id}", id);
            return ApiResponse<ExpenseDto>.Fail("حدث خطأ أثناء تحديث المصروف");
        }
    }

    public async Task<ApiResponse<string>> DeleteAsync(int id)
    {
        try
        {
            var entity = await _expenseRepo.GetByIdAsync(id);
            if (entity == null || entity.IsDeleted)
                return ApiResponse<string>.Fail("المصروف غير موجود");

            _expenseRepo.SoftDelete(entity);
            await _expenseRepo.SaveChangesAsync();

            return ApiResponse<string>.Ok(string.Empty, "تم حذف المصروف بنجاح");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في حذف المصروف {Id}", id);
            return ApiResponse<string>.Fail("حدث خطأ أثناء حذف المصروف");
        }
    }
}
