using EdoxoPro.Domain.Enums;

namespace EdoxoPro.Domain.Entities;

public class StockMovement : BaseEntity
{
    public int ProductId { get; set; }
    public int WarehouseId { get; set; }
    public double Quantity { get; set; }
    public StockMovementType Type { get; set; }
    public int? ReferenceId { get; set; }
    public string? ReferenceType { get; set; }
    public DateTime Date { get; set; } = DateTime.UtcNow;
    public string? Notes { get; set; }

    public Product Product { get; set; } = null!;
    public Warehouse Warehouse { get; set; } = null!;
}
