using AutoMapper;
using EdoxoPro.Application.Common;
using EdoxoPro.Application.DTOs.Settings;
using EdoxoPro.Application.Interfaces;
using EdoxoPro.Domain.Entities;
using EdoxoPro.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace EdoxoPro.Application.Services;

public class BarcodeSettingService : IBarcodeSettingService
{
    private readonly IGenericRepository<BarcodeSetting> _repo;
    private readonly IMapper _mapper;
    private readonly ILogger<BarcodeSettingService> _logger;

    public BarcodeSettingService(
        IGenericRepository<BarcodeSetting> repo,
        IMapper mapper,
        ILogger<BarcodeSettingService> logger)
    {
        _repo = repo;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ApiResponse<BarcodeSettingDto>> GetAsync()
    {
        try
        {
            var entities = await _repo.GetAllAsync();
            var entity = entities.FirstOrDefault();
            if (entity == null)
                return ApiResponse<BarcodeSettingDto>.Fail("إعدادات الباركود غير موجودة");

            var dto = _mapper.Map<BarcodeSettingDto>(entity);
            return ApiResponse<BarcodeSettingDto>.Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في جلب إعدادات الباركود");
            return ApiResponse<BarcodeSettingDto>.Fail("حدث خطأ أثناء جلب إعدادات الباركود");
        }
    }

    public async Task<ApiResponse<BarcodeSettingDto>> UpdateAsync(UpdateBarcodeSettingDto request)
    {
        try
        {
            var entities = await _repo.GetAllAsync();
            var entity = entities.FirstOrDefault();

            if (entity == null)
            {
                entity = _mapper.Map<BarcodeSetting>(request);
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

            var dto = _mapper.Map<BarcodeSettingDto>(entity);
            return ApiResponse<BarcodeSettingDto>.Ok(dto, "تم تحديث إعدادات الباركود بنجاح");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في تحديث إعدادات الباركود");
            return ApiResponse<BarcodeSettingDto>.Fail("حدث خطأ أثناء تحديث إعدادات الباركود");
        }
    }
}
