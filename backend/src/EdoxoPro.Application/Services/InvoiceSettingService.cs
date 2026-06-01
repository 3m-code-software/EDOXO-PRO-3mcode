using AutoMapper;
using EdoxoPro.Application.Common;
using EdoxoPro.Application.DTOs.Settings;
using EdoxoPro.Application.Interfaces;
using EdoxoPro.Domain.Entities;
using EdoxoPro.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace EdoxoPro.Application.Services;

public class InvoiceSettingService : IInvoiceSettingService
{
    private readonly IGenericRepository<InvoiceSetting> _repo;
    private readonly IMapper _mapper;
    private readonly ILogger<InvoiceSettingService> _logger;

    public InvoiceSettingService(
        IGenericRepository<InvoiceSetting> repo,
        IMapper mapper,
        ILogger<InvoiceSettingService> logger)
    {
        _repo = repo;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ApiResponse<InvoiceSettingDto>> GetAsync()
    {
        try
        {
            var entities = await _repo.GetAllAsync();
            var entity = entities.FirstOrDefault();
            if (entity == null)
                return ApiResponse<InvoiceSettingDto>.Fail("إعدادات الفاتورة غير موجودة");

            var dto = _mapper.Map<InvoiceSettingDto>(entity);
            return ApiResponse<InvoiceSettingDto>.Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في جلب إعدادات الفاتورة");
            return ApiResponse<InvoiceSettingDto>.Fail("حدث خطأ أثناء جلب إعدادات الفاتورة");
        }
    }

    public async Task<ApiResponse<InvoiceSettingDto>> UpdateAsync(UpdateInvoiceSettingDto request)
    {
        try
        {
            var entities = await _repo.GetAllAsync();
            var entity = entities.FirstOrDefault();

            if (entity == null)
            {
                entity = _mapper.Map<InvoiceSetting>(request);
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

            var dto = _mapper.Map<InvoiceSettingDto>(entity);
            return ApiResponse<InvoiceSettingDto>.Ok(dto, "تم تحديث إعدادات الفاتورة بنجاح");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في تحديث إعدادات الفاتورة");
            return ApiResponse<InvoiceSettingDto>.Fail("حدث خطأ أثناء تحديث إعدادات الفاتورة");
        }
    }
}
