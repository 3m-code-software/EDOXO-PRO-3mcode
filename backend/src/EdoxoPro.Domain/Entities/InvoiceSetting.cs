namespace EdoxoPro.Domain.Entities;

public class InvoiceSetting : BaseEntity
{
    public string Prefix { get; set; } = string.Empty;
    public int NextNumber { get; set; } = 1;
    public decimal TaxRate { get; set; }
    public string? Footer { get; set; }
    public bool ShowTax { get; set; } = true;
    public bool ShowDiscount { get; set; } = true;
}
