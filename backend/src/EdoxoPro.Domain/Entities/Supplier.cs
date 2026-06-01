namespace EdoxoPro.Domain.Entities;

public class Supplier : BaseEntity
{
    public string ContactId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? BusinessName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? TaxNumber { get; set; }
    public string? CommercialRegister { get; set; }
    public int? PaymentPeriod { get; set; }
    public decimal OpeningBalance { get; set; }
    public decimal? PreviousBalance { get; set; }
    public decimal? CreditLimit { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public string? Notes { get; set; }
    public bool IsActive { get; set; } = true;

    public string? CustomField1 { get; set; }
    public string? CustomField2 { get; set; }
    public string? CustomField3 { get; set; }
    public string? CustomField4 { get; set; }
    public string? CustomField5 { get; set; }
    public string? CustomField6 { get; set; }
    public string? CustomField7 { get; set; }
    public string? CustomField8 { get; set; }
    public string? CustomField9 { get; set; }
    public string? CustomField10 { get; set; }

    public ICollection<Purchase> Purchases { get; set; } = new List<Purchase>();
}
