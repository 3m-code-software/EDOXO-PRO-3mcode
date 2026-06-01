namespace EdoxoPro.Application.DTOs.Settings;
public class UpdateCompanySettingDto
{
    public string CompanyName { get; set; } = string.Empty;
    public string? CompanyNameAr { get; set; }
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Website { get; set; }
    public string? TaxNumber { get; set; }
    public string? CommercialRegister { get; set; }
    public string? LogoPath { get; set; }
    public string? CurrencyCode { get; set; }
}
