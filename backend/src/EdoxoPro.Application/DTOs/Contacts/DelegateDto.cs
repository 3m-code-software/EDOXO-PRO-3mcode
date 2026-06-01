namespace EdoxoPro.Application.DTOs.Contacts;

public class DelegateDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public decimal CommissionPercent { get; set; }
    public bool IsActive { get; set; }
}
