namespace EdoxoPro.Domain.Entities;

public class CustomerGroup : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal DiscountPercent { get; set; }
}
