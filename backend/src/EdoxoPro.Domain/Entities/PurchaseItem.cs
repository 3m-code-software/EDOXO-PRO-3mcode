namespace EdoxoPro.Domain.Entities;

public class PurchaseItem : BaseEntity
{
    public int PurchaseId { get; set; }
    public int ProductId { get; set; }
    public int? VariantId { get; set; }
    public double Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Total { get; set; }

    public Purchase Purchase { get; set; } = null!;
    public Product Product { get; set; } = null!;
}
