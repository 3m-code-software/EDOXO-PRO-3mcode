namespace EdoxoPro.Domain.Entities;

public class PurchaseReturnItem : BaseEntity
{
    public int ReturnId { get; set; }
    public int PurchaseItemId { get; set; }
    public double Quantity { get; set; }
    public decimal UnitPrice { get; set; }

    public PurchaseReturn Return { get; set; } = null!;
    public PurchaseItem PurchaseItem { get; set; } = null!;
}
