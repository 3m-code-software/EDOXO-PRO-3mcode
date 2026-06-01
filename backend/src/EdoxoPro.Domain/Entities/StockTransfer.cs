namespace EdoxoPro.Domain.Entities;

public class StockTransfer : BaseEntity
{
    public string TransferNumber { get; set; } = string.Empty;
    public int FromWarehouseId { get; set; }
    public int ToWarehouseId { get; set; }
    public DateTime Date { get; set; } = DateTime.UtcNow;
    public string Status { get; set; } = string.Empty;
    public string? Notes { get; set; }

    public Warehouse FromWarehouse { get; set; } = null!;
    public Warehouse ToWarehouse { get; set; } = null!;
    public ICollection<StockTransferItem> Items { get; set; } = new List<StockTransferItem>();
}
