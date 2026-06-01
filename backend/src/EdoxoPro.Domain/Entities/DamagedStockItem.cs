namespace EdoxoPro.Domain.Entities;

public class DamagedStockItem : BaseEntity
{
    public int DamagedStockId { get; set; }
    public int ProductId { get; set; }
    public double Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Total { get; set; }

    public DamagedStock DamagedStock { get; set; } = null!;
    public Product Product { get; set; } = null!;
}
