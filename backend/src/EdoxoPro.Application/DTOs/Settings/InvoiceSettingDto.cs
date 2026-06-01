namespace EdoxoPro.Application.DTOs.Settings;
public class InvoiceSettingDto
{
    public int Id { get; set; }
    public string? DefaultNotes { get; set; }
    public string? DefaultTerms { get; set; }
    public bool ShowDiscount { get; set; }
    public bool ShowTax { get; set; }
    public bool ShowShipping { get; set; }
    public int DueDays { get; set; }
    public string? FooterText { get; set; }
}
