namespace EdoxoPro.Application.DTOs.Contacts;

public class CreateCustomerDto
{
    public string Name { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? TaxNumber { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public decimal? OpeningBalance { get; set; }
    public int? CustomerGroupId { get; set; }
}
