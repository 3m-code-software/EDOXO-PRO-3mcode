using EdoxoPro.Application.Common;
using EdoxoPro.Application.DTOs.Contacts;

namespace EdoxoPro.Application.Interfaces;
public interface ICustomerService
{
    Task<ApiResponse<PagedResult<CustomerDto>>> GetAllAsync(FilterRequest request);
    Task<ApiResponse<CustomerDto>> GetByIdAsync(int id);
    Task<ApiResponse<CustomerDto>> CreateAsync(CreateCustomerDto request);
    Task<ApiResponse<CustomerDto>> UpdateAsync(int id, UpdateCustomerDto request);
    Task<ApiResponse<string>> DeleteAsync(int id);
}
