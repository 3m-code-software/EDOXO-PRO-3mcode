namespace EdoxoPro.Application.DTOs.Contacts;

public class UpdateDelegateDto
{
    public string? Title { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public decimal CommissionPercent { get; set; }
}
