namespace EdoxoPro.Domain.Entities;

public class Customer : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? TaxNumber { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public decimal OpeningBalance { get; set; }
    public int? GroupId { get; set; }
    public bool IsActive { get; set; } = true;

    public CustomerGroup? CustomerGroup { get; set; }
    public ICollection<Sale> Sales { get; set; } = new List<Sale>();
}
