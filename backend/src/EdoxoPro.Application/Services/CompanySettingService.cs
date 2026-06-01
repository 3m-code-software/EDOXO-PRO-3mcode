using AutoMapper;
using EdoxoPro.Application.Common;
using EdoxoPro.Application.DTOs.Settings;
using EdoxoPro.Application.Interfaces;
using EdoxoPro.Domain.Entities;
using EdoxoPro.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace EdoxoPro.Application.Services;

public class CompanySettingService : ICompanySettingService
{
    private readonly IGenericRepository<CompanySetting> _repo;
    private readonly IMapper _mapper;
    private readonly ILogger<CompanySettingService> _logger;

    public CompanySettingService(
        IGenericRepository<CompanySetting> repo,
        IMapper mapper,
        ILogger<CompanySettingService> logger)
    {
        _repo = repo;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ApiResponse<CompanySettingDto>> GetAsync()
    {
        try
        {
            var entities = await _repo.GetAllAsync();
            var entity = entities.FirstOrDefault();
            if (entity == null)
                return ApiResponse<CompanySettingDto>.Fail("إعدادات الشركة غير موجودة");

            var dto = _mapper.Map<CompanySettingDto>(entity);
            return ApiResponse<CompanySettingDto>.Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في جلب إعدادات الشركة");
            return ApiResponse<CompanySettingDto>.Fail("حدث خطأ أثناء جلب إعدادات الشركة");
        }
    }

    public async Task<ApiResponse<CompanySettingDto>> UpdateAsync(UpdateCompanySettingDto request)
    {
        try
        {
            var entities = await _repo.GetAllAsync();
            var entity = entities.FirstOrDefault();

            if (entity == null)
            {
                entity = _mapper.Map<CompanySetting>(request);
                entity.CreatedAt = DateTime.UtcNow;
                await _repo.AddAsync(entity);
            }
            else
            {
                _mapper.Map(request, entity);
                entity.UpdatedAt = DateTime.UtcNow;
                _repo.Update(entity);
            }

            await _repo.SaveChangesAsync();

            var dto = _mapper.Map<CompanySettingDto>(entity);
            return ApiResponse<CompanySettingDto>.Ok(dto, "تم تحديث إعدادات الشركة بنجاح");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في تحديث إعدادات الشركة");
            return ApiResponse<CompanySettingDto>.Fail("حدث خطأ أثناء تحديث إعدادات الشركة");
        }
    }
}
