namespace EdoxoPro.Domain.Entities;

public class DamagedStock : BaseEntity
{
    public string ReferenceNumber { get; set; } = string.Empty;
    public int WarehouseId { get; set; }
    public DateTime Date { get; set; } = DateTime.UtcNow;
    public string? Reason { get; set; }
    public decimal Total { get; set; }
    public string? Notes { get; set; }

    public Warehouse Warehouse { get; set; } = null!;
    public ICollection<DamagedStockItem> Items { get; set; } = new List<DamagedStockItem>();
}
