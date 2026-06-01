namespace EdoxoPro.Application.DTOs.Checks;
public class UpdateCheckDto
{
    public string? CheckNumber { get; set; }
    public decimal? Amount { get; set; }
    public string? BankName { get; set; }
    public DateTime? DueDate { get; set; }
    public string? Notes { get; set; }
    public string? OwnerName { get; set; }
}
