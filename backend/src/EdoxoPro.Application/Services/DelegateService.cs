using AutoMapper;
using EdoxoPro.Application.Common;
using EdoxoPro.Application.DTOs.Contacts;
using EdoxoPro.Application.Interfaces;
using EdoxoPro.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace EdoxoPro.Application.Services;

public class DelegateService : IDelegateService
{
    private readonly IGenericRepository<Domain.Entities.Delegate> _delegateRepo;
    private readonly IMapper _mapper;
    private readonly ILogger<DelegateService> _logger;

    public DelegateService(IGenericRepository<Domain.Entities.Delegate> delegateRepo, IMapper mapper, ILogger<DelegateService> logger)
    {
        _delegateRepo = delegateRepo;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ApiResponse<PagedResult<DelegateDto>>> GetAllAsync(DelegateFilterRequest request)
    {
        try
        {
            var items = await _delegateRepo.FindAsync(d => !d.IsDeleted);
            var query = items.AsQueryable();
            if (request.IsActive.HasValue)
                query = query.Where(d => d.IsActive == request.IsActive.Value);
            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var s = request.Search.ToLower();
                query = query.Where(d => d.FirstName.ToLower().Contains(s) || d.LastName.ToLower().Contains(s) || (d.Email != null && d.Email.ToLower().Contains(s)) || (d.Phone != null && d.Phone.Contains(s)));
            }
            var total = query.Count();
            var list = query.OrderByDescending(d => d.Id).Skip((request.Page - 1) * request.PageSize).Take(request.PageSize).ToList();
            var dtos = _mapper.Map<List<DelegateDto>>(list);
            var result = new PagedResult<DelegateDto> { Items = dtos, TotalCount = total, Page = request.Page, PageSize = request.PageSize };
            return ApiResponse<PagedResult<DelegateDto>>.Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في جلب قائمة المندوبين");
            return ApiResponse<PagedResult<DelegateDto>>.Fail("حدث خطأ أثناء جلب المندوبين");
        }
    }

    public async Task<ApiResponse<DelegateDto>> GetByIdAsync(int id)
    {
        try
        {
            var entity = await _delegateRepo.GetByIdAsync(id);
            if (entity == null || entity.IsDeleted) return ApiResponse<DelegateDto>.Fail("المندوب غير موجود");
            return ApiResponse<DelegateDto>.Ok(_mapper.Map<DelegateDto>(entity));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في جلب المندوب {Id}", id);
            return ApiResponse<DelegateDto>.Fail("حدث خطأ أثناء جلب المندوب");
        }
    }

    public async Task<ApiResponse<DelegateDto>> CreateAsync(CreateDelegateDto request)
    {
        try
        {
            var entity = _mapper.Map<Domain.Entities.Delegate>(request);
            entity.CreatedAt = DateTime.UtcNow;
            entity.IsActive = true;
            await _delegateRepo.AddAsync(entity);
            await _delegateRepo.SaveChangesAsync();
            return ApiResponse<DelegateDto>.Ok(_mapper.Map<DelegateDto>(entity), "تم إنشاء المندوب بنجاح");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في إنشاء مندوب جديد");
            return ApiResponse<DelegateDto>.Fail("حدث خطأ أثناء إنشاء المندوب");
        }
    }

    public async Task<ApiResponse<DelegateDto>> UpdateAsync(int id, UpdateDelegateDto request)
    {
        try
        {
            var entity = await _delegateRepo.GetByIdAsync(id);
            if (entity == null || entity.IsDeleted) return ApiResponse<DelegateDto>.Fail("المندوب غير موجود");
            _mapper.Map(request, entity);
            entity.UpdatedAt = DateTime.UtcNow;
            _delegateRepo.Update(entity);
            await _delegateRepo.SaveChangesAsync();
            return ApiResponse<DelegateDto>.Ok(_mapper.Map<DelegateDto>(entity), "تم تحديث المندوب بنجاح");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في تحديث المندوب {Id}", id);
            return ApiResponse<DelegateDto>.Fail("حدث خطأ أثناء تحديث المندوب");
        }
    }

    public async Task<ApiResponse<string>> DeleteAsync(int id)
    {
        try
        {
            var entity = await _delegateRepo.GetByIdAsync(id);
            if (entity == null || entity.IsDeleted) return ApiResponse<string>.Fail("المندوب غير موجود");
            _delegateRepo.SoftDelete(entity);
            await _delegateRepo.SaveChangesAsync();
            return ApiResponse<string>.Ok(string.Empty, "تم حذف المندوب بنجاح");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في حذف المندوب {Id}", id);
            return ApiResponse<string>.Fail("حدث خطأ أثناء حذف المندوب");
        }
    }
}
