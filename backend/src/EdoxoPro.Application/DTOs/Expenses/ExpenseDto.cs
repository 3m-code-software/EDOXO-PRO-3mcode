namespace EdoxoPro.Application.DTOs.Expenses;

public class ExpenseDto
{
    public int Id { get; set; }
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public string? Description { get; set; }
    public int? BranchId { get; set; }
    public string? PaymentMethod { get; set; }
}
