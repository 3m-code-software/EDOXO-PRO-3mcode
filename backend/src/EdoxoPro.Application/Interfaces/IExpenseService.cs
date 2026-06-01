using EdoxoPro.Application.Common;
using EdoxoPro.Application.DTOs.Expenses;

namespace EdoxoPro.Application.Interfaces;
public interface IExpenseService
{
    Task<ApiResponse<PagedResult<ExpenseDto>>> GetAllAsync(FilterRequest request);
    Task<ApiResponse<ExpenseDto>> GetByIdAsync(int id);
    Task<ApiResponse<ExpenseDto>> CreateAsync(CreateExpenseDto request);
    Task<ApiResponse<ExpenseDto>> UpdateAsync(int id, UpdateExpenseDto request);
    Task<ApiResponse<string>> DeleteAsync(int id);
}
