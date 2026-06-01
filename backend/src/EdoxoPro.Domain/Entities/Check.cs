using EdoxoPro.Domain.Enums;

namespace EdoxoPro.Domain.Entities;

public class Check : BaseEntity
{
    public string CheckNumber { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string? BankName { get; set; }
    public DateTime IssueDate { get; set; } = DateTime.UtcNow;
    public DateTime DueDate { get; set; }
    public CheckStatus Status { get; set; } = CheckStatus.Pending;
    public string Type { get; set; } = string.Empty;
    public int? ReferenceId { get; set; }
    public string? ReferenceType { get; set; }
    public string? Notes { get; set; }
    public string? OwnerName { get; set; }
}
