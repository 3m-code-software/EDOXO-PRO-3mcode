using AutoMapper;
using EdoxoPro.Application.Common;
using EdoxoPro.Application.DTOs.Settings;
using EdoxoPro.Application.Interfaces;
using EdoxoPro.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace EdoxoPro.Application.Services;

public class BranchService : IBranchService
{
    private readonly IGenericRepository<Branch> _repo;
    private readonly IMapper _mapper;
    private readonly ILogger<BranchService> _logger;

    public BranchService(
        IGenericRepository<Branch> repo,
        IMapper mapper,
        ILogger<BranchService> logger)
    {
        _repo = repo;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ApiResponse<IEnumerable<BranchDto>>> GetAllAsync()
    {
        try
        {
            var entities = await _repo.GetAllAsync();
            var dtos = _mapper.Map<List<BranchDto>>(entities.Where(e => !e.IsDeleted));
            return ApiResponse<IEnumerable<BranchDto>>.Ok(dtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في جلب الفروع");
            return ApiResponse<IEnumerable<BranchDto>>.Fail("حدث خطأ أثناء جلب الفروع");
        }
    }

    public async Task<ApiResponse<BranchDto>> GetByIdAsync(int id)
    {
        try
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null || entity.IsDeleted)
                return ApiResponse<BranchDto>.Fail("الفرع غير موجود");

            var dto = _mapper.Map<BranchDto>(entity);
            return ApiResponse<BranchDto>.Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في جلب الفرع {Id}", id);
            return ApiResponse<BranchDto>.Fail("حدث خطأ أثناء جلب الفرع");
        }
    }

    public async Task<ApiResponse<BranchDto>> CreateAsync(CreateBranchDto request)
    {
        try
        {
            var entity = _mapper.Map<Branch>(request);
            entity.CreatedAt = DateTime.UtcNow;
            entity.IsActive = true;

            await _repo.AddAsync(entity);
            await _repo.SaveChangesAsync();

            var dto = _mapper.Map<BranchDto>(entity);
            return ApiResponse<BranchDto>.Ok(dto, "تم إنشاء الفرع بنجاح");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في إنشاء فرع جديد");
            return ApiResponse<BranchDto>.Fail("حدث خطأ أثناء إنشاء الفرع");
        }
    }

    public async Task<ApiResponse<BranchDto>> UpdateAsync(int id, UpdateBranchDto request)
    {
        try
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null || entity.IsDeleted)
                return ApiResponse<BranchDto>.Fail("الفرع غير موجود");

            _mapper.Map(request, entity);
            entity.UpdatedAt = DateTime.UtcNow;
            _repo.Update(entity);
            await _repo.SaveChangesAsync();

            var dto = _mapper.Map<BranchDto>(entity);
            return ApiResponse<BranchDto>.Ok(dto, "تم تحديث الفرع بنجاح");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في تحديث الفرع {Id}", id);
            return ApiResponse<BranchDto>.Fail("حدث خطأ أثناء تحديث الفرع");
        }
    }

    public async Task<ApiResponse<string>> DeleteAsync(int id)
    {
        try
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null || entity.IsDeleted)
                return ApiResponse<string>.Fail("الفرع غير موجود");

            _repo.SoftDelete(entity);
            await _repo.SaveChangesAsync();

            return ApiResponse<string>.Ok(string.Empty, "تم حذف الفرع بنجاح");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في حذف الفرع {Id}", id);
            return ApiResponse<string>.Fail("حدث خطأ أثناء حذف الفرع");
        }
    }
}
