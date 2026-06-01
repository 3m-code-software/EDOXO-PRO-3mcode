namespace EdoxoPro.Domain.Entities;

public class Expense : BaseEntity
{
    public int CategoryId { get; set; }
    public decimal Amount { get; set; }
    public DateTime Date { get; set; } = DateTime.UtcNow;
    public string? Description { get; set; }
    public int? BranchId { get; set; }
    public int AddedByUserId { get; set; }
    public string? PaymentMethod { get; set; }

    public ExpenseCategory Category { get; set; } = null!;
}
