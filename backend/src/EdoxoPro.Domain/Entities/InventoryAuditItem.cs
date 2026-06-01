namespace EdoxoPro.Domain.Entities;

public class InventoryAuditItem : BaseEntity
{
    public int AuditId { get; set; }
    public int ProductId { get; set; }
    public double SystemQuantity { get; set; }
    public double ActualQuantity { get; set; }
    public double Difference { get; set; }
    public decimal UnitPrice { get; set; }

    public InventoryAudit Audit { get; set; } = null!;
    public Product Product { get; set; } = null!;
}
