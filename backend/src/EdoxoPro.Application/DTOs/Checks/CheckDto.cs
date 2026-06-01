namespace EdoxoPro.Application.DTOs.Checks;

public class CheckDto
{
    public int Id { get; set; }
    public string CheckNumber { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string? BankName { get; set; }
    public DateTime IssueDate { get; set; }
    public DateTime DueDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public int? ReferenceId { get; set; }
    public string? ReferenceType { get; set; }
    public string? Notes { get; set; }
    public string? OwnerName { get; set; }
}
