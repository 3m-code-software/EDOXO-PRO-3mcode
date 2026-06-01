namespace EdoxoPro.Domain.Entities;

public class SaleReturnItem : BaseEntity
{
    public int ReturnId { get; set; }
    public int SaleItemId { get; set; }
    public double Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Total { get; set; }

    public SaleReturn Return { get; set; } = null!;
    public SaleItem SaleItem { get; set; } = null!;
}
