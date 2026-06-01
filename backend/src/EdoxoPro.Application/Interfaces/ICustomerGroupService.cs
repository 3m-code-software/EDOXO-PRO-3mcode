using EdoxoPro.Application.Common;
using EdoxoPro.Application.DTOs.Contacts;

namespace EdoxoPro.Application.Interfaces;
public interface ICustomerGroupService
{
    Task<ApiResponse<IEnumerable<CustomerGroupDto>>> GetAllAsync();
    Task<ApiResponse<CustomerGroupDto>> GetByIdAsync(int id);
    Task<ApiResponse<CustomerGroupDto>> CreateAsync(CreateCustomerGroupDto request);
    Task<ApiResponse<CustomerGroupDto>> UpdateAsync(int id, UpdateCustomerGroupDto request);
    Task<ApiResponse<string>> DeleteAsync(int id);
}
