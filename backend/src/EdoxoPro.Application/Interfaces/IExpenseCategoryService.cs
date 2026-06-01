using EdoxoPro.Application.Common;
using EdoxoPro.Application.DTOs.Expenses;

namespace EdoxoPro.Application.Interfaces;
public interface IExpenseCategoryService
{
    Task<ApiResponse<IEnumerable<ExpenseCategoryDto>>> GetAllAsync();
    Task<ApiResponse<ExpenseCategoryDto>> GetByIdAsync(int id);
    Task<ApiResponse<ExpenseCategoryDto>> CreateAsync(CreateExpenseCategoryDto request);
    Task<ApiResponse<ExpenseCategoryDto>> UpdateAsync(int id, UpdateExpenseCategoryDto request);
    Task<ApiResponse<string>> DeleteAsync(int id);
}
