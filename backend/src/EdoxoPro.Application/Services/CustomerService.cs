using AutoMapper;
using EdoxoPro.Application.Common;
using EdoxoPro.Application.DTOs.Contacts;
using EdoxoPro.Application.Interfaces;
using EdoxoPro.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace EdoxoPro.Application.Services;

public class CustomerService : ICustomerService
{
    private readonly IGenericRepository<Customer> _customerRepo;
    private readonly IGenericRepository<CustomerGroup> _groupRepo;
    private readonly IMapper _mapper;
    private readonly ILogger<CustomerService> _logger;

    public CustomerService(
        IGenericRepository<Customer> customerRepo,
        IGenericRepository<CustomerGroup> groupRepo,
        IMapper mapper,
        ILogger<CustomerService> logger)
    {
        _customerRepo = customerRepo;
        _groupRepo = groupRepo;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ApiResponse<PagedResult<CustomerDto>>> GetAllAsync(FilterRequest request)
    {
        try
        {
            var result = await _customerRepo.GetPagedAsync(request);
            var items = _mapper.Map<List<CustomerDto>>(result.Items);

            foreach (var dto in items)
            {
                if (dto.GroupId.HasValue)
                {
                    var group = await _groupRepo.GetByIdAsync(dto.GroupId.Value);
                    dto.GroupName = group?.Name;
                }
            }

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var s = request.Search.ToLower();
                items = items.Where(i => i.Name.ToLower().Contains(s)).ToList();
            }

            var pagedResult = new PagedResult<CustomerDto>
            {
                Items = items,
                TotalCount = result.TotalCount,
                Page = result.Page,
                PageSize = result.PageSize
            };

            return ApiResponse<PagedResult<CustomerDto>>.Ok(pagedResult);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في جلب قائمة العملاء");
            return ApiResponse<PagedResult<CustomerDto>>.Fail("حدث خطأ أثناء جلب العملاء");
        }
    }

    public async Task<ApiResponse<CustomerDto>> GetByIdAsync(int id)
    {
        try
        {
            var customer = await _customerRepo.GetByIdAsync(id);
            if (customer == null)
                return ApiResponse<CustomerDto>.Fail("العميل غير موجود");

            var dto = _mapper.Map<CustomerDto>(customer);
            if (customer.GroupId.HasValue)
            {
                var group = await _groupRepo.GetByIdAsync(customer.GroupId.Value);
                dto.GroupName = group?.Name;
            }

            return ApiResponse<CustomerDto>.Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في جلب العميل {CustomerId}", id);
            return ApiResponse<CustomerDto>.Fail("حدث خطأ أثناء جلب العميل");
        }
    }

    public async Task<ApiResponse<CustomerDto>> CreateAsync(CreateCustomerDto request)
    {
        try
        {
            var customer = _mapper.Map<Customer>(request);
            customer.CreatedAt = DateTime.UtcNow;
            customer.IsActive = true;

            await _customerRepo.AddAsync(customer);

            var dto = _mapper.Map<CustomerDto>(customer);
            if (customer.GroupId.HasValue)
            {
                var group = await _groupRepo.GetByIdAsync(customer.GroupId.Value);
                dto.GroupName = group?.Name;
            }

            return ApiResponse<CustomerDto>.Ok(dto, "تم إنشاء العميل بنجاح");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في إنشاء عميل جديد");
            return ApiResponse<CustomerDto>.Fail("حدث خطأ أثناء إنشاء العميل");
        }
    }

    public async Task<ApiResponse<CustomerDto>> UpdateAsync(int id, UpdateCustomerDto request)
    {
        try
        {
            var customer = await _customerRepo.GetByIdAsync(id);
            if (customer == null)
                return ApiResponse<CustomerDto>.Fail("العميل غير موجود");

            _mapper.Map(request, customer);
            customer.UpdatedAt = DateTime.UtcNow;
            _customerRepo.Update(customer);

            var dto = _mapper.Map<CustomerDto>(customer);
            if (customer.GroupId.HasValue)
            {
                var group = await _groupRepo.GetByIdAsync(customer.GroupId.Value);
                dto.GroupName = group?.Name;
            }

            return ApiResponse<CustomerDto>.Ok(dto, "تم تحديث العميل بنجاح");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في تحديث العميل {CustomerId}", id);
            return ApiResponse<CustomerDto>.Fail("حدث خطأ أثناء تحديث العميل");
        }
    }

    public async Task<ApiResponse<string>> DeleteAsync(int id)
    {
        try
        {
            var customer = await _customerRepo.GetByIdAsync(id);
            if (customer == null)
                return ApiResponse<string>.Fail("العميل غير موجود");

            _customerRepo.SoftDelete(customer);
            return ApiResponse<string>.Ok(string.Empty, "تم حذف العميل بنجاح");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في حذف العميل {CustomerId}", id);
            return ApiResponse<string>.Fail("حدث خطأ أثناء حذف العميل");
        }
    }
}
