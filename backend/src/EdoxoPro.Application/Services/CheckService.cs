using AutoMapper;
using EdoxoPro.Application.Common;
using EdoxoPro.Application.DTOs.Checks;
using EdoxoPro.Application.Interfaces;
using EdoxoPro.Domain.Entities;
using EdoxoPro.Domain.Enums;
using EdoxoPro.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace EdoxoPro.Application.Services;

public class CheckService : ICheckService
{
    private readonly IGenericRepository<Check> _checkRepo;
    private readonly IMapper _mapper;
    private readonly ILogger<CheckService> _logger;

    private static readonly Dictionary<CheckStatus, List<CheckStatus>> ValidTransitions = new()
    {
        { CheckStatus.UnderCollection, new() { CheckStatus.Collected, CheckStatus.Returned, CheckStatus.Cancelled } },
        { CheckStatus.Collected, new() { CheckStatus.Returned } },
        { CheckStatus.Returned, new() },
        { CheckStatus.Cancelled, new() }
    };

    public CheckService(
        IGenericRepository<Check> checkRepo,
        IMapper mapper,
        ILogger<CheckService> logger)
    {
        _checkRepo = checkRepo;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ApiResponse<PagedResult<CheckDto>>> GetAllAsync(FilterRequest request)
    {
        try
        {
            var items = await _checkRepo.FindAsync(c => !c.IsDeleted);
            var query = items.AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var s = request.Search.ToLower();
                query = query.Where(c => c.CheckNumber.ToLower().Contains(s) ||
                                         (c.BankName != null && c.BankName.ToLower().Contains(s)) ||
                                         (c.OwnerName != null && c.OwnerName.ToLower().Contains(s)));
            }

            var total = query.Count();
            var list = query.OrderByDescending(c => c.IssueDate)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            var dtos = _mapper.Map<List<CheckDto>>(list);
            var result = new PagedResult<CheckDto>
            {
                Items = dtos,
                TotalCount = total,
                Page = request.Page,
                PageSize = request.PageSize
            };

            return ApiResponse<PagedResult<CheckDto>>.Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في جلب الشيكات");
            return ApiResponse<PagedResult<CheckDto>>.Fail("حدث خطأ أثناء جلب الشيكات");
        }
    }

    public async Task<ApiResponse<CheckDto>> GetByIdAsync(int id)
    {
        try
        {
            var entity = await _checkRepo.GetByIdAsync(id);
            if (entity == null || entity.IsDeleted)
                return ApiResponse<CheckDto>.Fail("الشيك غير موجود");

            var dto = _mapper.Map<CheckDto>(entity);
            return ApiResponse<CheckDto>.Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في جلب الشيك {Id}", id);
            return ApiResponse<CheckDto>.Fail("حدث خطأ أثناء جلب الشيك");
        }
    }

    public async Task<ApiResponse<CheckDto>> CreateAsync(CreateCheckDto request)
    {
        try
        {
            var entity = _mapper.Map<Check>(request);
            entity.Status = CheckStatus.UnderCollection;
            entity.CreatedAt = DateTime.UtcNow;

            await _checkRepo.AddAsync(entity);
            await _checkRepo.SaveChangesAsync();

            var dto = _mapper.Map<CheckDto>(entity);
            return ApiResponse<CheckDto>.Ok(dto, "تم إنشاء الشيك بنجاح");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في إنشاء شيك جديد");
            return ApiResponse<CheckDto>.Fail("حدث خطأ أثناء إنشاء الشيك");
        }
    }

    public async Task<ApiResponse<CheckDto>> UpdateAsync(int id, UpdateCheckDto request)
    {
        try
        {
            var entity = await _checkRepo.GetByIdAsync(id);
            if (entity == null || entity.IsDeleted)
                return ApiResponse<CheckDto>.Fail("الشيك غير موجود");

            _mapper.Map(request, entity);
            entity.UpdatedAt = DateTime.UtcNow;
            _checkRepo.Update(entity);
            await _checkRepo.SaveChangesAsync();

            var dto = _mapper.Map<CheckDto>(entity);
            return ApiResponse<CheckDto>.Ok(dto, "تم تحديث الشيك بنجاح");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في تحديث الشيك {Id}", id);
            return ApiResponse<CheckDto>.Fail("حدث خطأ أثناء تحديث الشيك");
        }
    }

    public async Task<ApiResponse<string>> DeleteAsync(int id)
    {
        try
        {
            var entity = await _checkRepo.GetByIdAsync(id);
            if (entity == null || entity.IsDeleted)
                return ApiResponse<string>.Fail("الشيك غير موجود");

            _checkRepo.SoftDelete(entity);
            await _checkRepo.SaveChangesAsync();

            return ApiResponse<string>.Ok(string.Empty, "تم حذف الشيك بنجاح");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في حذف الشيك {Id}", id);
            return ApiResponse<string>.Fail("حدث خطأ أثناء حذف الشيك");
        }
    }

    public async Task<ApiResponse<string>> UpdateStatusAsync(int id, UpdateCheckStatusDto request)
    {
        try
        {
            var entity = await _checkRepo.GetByIdAsync(id);
            if (entity == null || entity.IsDeleted)
                return ApiResponse<string>.Fail("الشيك غير موجود");

            if (!ValidTransitions.ContainsKey(entity.Status))
                return ApiResponse<string>.Fail("لا يمكن تغيير حالة الشيك في حالته الحالية");

            var allowed = ValidTransitions[entity.Status];
            if (!allowed.Contains(request.Status))
                return ApiResponse<string>.Fail("لا يمكن تحويل الشيك من {entity.Status} إلى {request.Status}");

            entity.Status = request.Status;
            entity.UpdatedAt = DateTime.UtcNow;
            _checkRepo.Update(entity);
            await _checkRepo.SaveChangesAsync();

            return ApiResponse<string>.Ok(string.Empty, "تم تحديث حالة الشيك بنجاح");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في تحديث حالة الشيك {Id}", id);
            return ApiResponse<string>.Fail("حدث خطأ أثناء تحديث حالة الشيك");
        }
    }
}
