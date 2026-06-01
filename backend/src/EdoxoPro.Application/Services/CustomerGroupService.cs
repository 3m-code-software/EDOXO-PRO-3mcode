using AutoMapper;
using EdoxoPro.Application.Common;
using EdoxoPro.Application.DTOs.Contacts;
using EdoxoPro.Application.Interfaces;
using EdoxoPro.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace EdoxoPro.Application.Services;

public class CustomerGroupService : ICustomerGroupService
{
    private readonly IGenericRepository<CustomerGroup> _groupRepo;
    private readonly IMapper _mapper;
    private readonly ILogger<CustomerGroupService> _logger;

    public CustomerGroupService(
        IGenericRepository<CustomerGroup> groupRepo,
        IMapper mapper,
        ILogger<CustomerGroupService> logger)
    {
        _groupRepo = groupRepo;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ApiResponse<IEnumerable<CustomerGroupDto>>> GetAllAsync()
    {
        try
        {
            var groups = await _groupRepo.GetAllAsync();
            var dtos = _mapper.Map<List<CustomerGroupDto>>(groups);
            return ApiResponse<IEnumerable<CustomerGroupDto>>.Ok(dtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في جلب مجموعات العملاء");
            return ApiResponse<IEnumerable<CustomerGroupDto>>.Fail("حدث خطأ أثناء جلب المجموعات");
        }
    }

    public async Task<ApiResponse<CustomerGroupDto>> GetByIdAsync(int id)
    {
        try
        {
            var group = await _groupRepo.GetByIdAsync(id);
            if (group == null)
                return ApiResponse<CustomerGroupDto>.Fail("المجموعة غير موجودة");

            var dto = _mapper.Map<CustomerGroupDto>(group);
            return ApiResponse<CustomerGroupDto>.Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في جلب المجموعة {GroupId}", id);
            return ApiResponse<CustomerGroupDto>.Fail("حدث خطأ أثناء جلب المجموعة");
        }
    }

    public async Task<ApiResponse<CustomerGroupDto>> CreateAsync(CreateCustomerGroupDto request)
    {
        try
        {
            var group = _mapper.Map<CustomerGroup>(request);
            group.CreatedAt = DateTime.UtcNow;

            await _groupRepo.AddAsync(group);

            var dto = _mapper.Map<CustomerGroupDto>(group);
            return ApiResponse<CustomerGroupDto>.Ok(dto, "تم إنشاء المجموعة بنجاح");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في إنشاء مجموعة جديدة");
            return ApiResponse<CustomerGroupDto>.Fail("حدث خطأ أثناء إنشاء المجموعة");
        }
    }

    public async Task<ApiResponse<CustomerGroupDto>> UpdateAsync(int id, UpdateCustomerGroupDto request)
    {
        try
        {
            var group = await _groupRepo.GetByIdAsync(id);
            if (group == null)
                return ApiResponse<CustomerGroupDto>.Fail("المجموعة غير موجودة");

            _mapper.Map(request, group);
            group.UpdatedAt = DateTime.UtcNow;
            _groupRepo.Update(group);

            var dto = _mapper.Map<CustomerGroupDto>(group);
            return ApiResponse<CustomerGroupDto>.Ok(dto, "تم تحديث المجموعة بنجاح");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في تحديث المجموعة {GroupId}", id);
            return ApiResponse<CustomerGroupDto>.Fail("حدث خطأ أثناء تحديث المجموعة");
        }
    }

    public async Task<ApiResponse<string>> DeleteAsync(int id)
    {
        try
        {
            var group = await _groupRepo.GetByIdAsync(id);
            if (group == null)
                return ApiResponse<string>.Fail("المجموعة غير موجودة");

            _groupRepo.SoftDelete(group);
            return ApiResponse<string>.Ok(string.Empty, "تم حذف المجموعة بنجاح");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في حذف المجموعة {GroupId}", id);
            return ApiResponse<string>.Fail("حدث خطأ أثناء حذف المجموعة");
        }
    }
}
