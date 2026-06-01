using EdoxoPro.Domain.Enums;

namespace EdoxoPro.Domain.Entities;

public class InventoryAudit : BaseEntity
{
    public string AuditNumber { get; set; } = string.Empty;
    public int WarehouseId { get; set; }
    public DateTime Date { get; set; } = DateTime.UtcNow;
    public InventoryAuditStatus Status { get; set; } = InventoryAuditStatus.Pending;
    public string? Notes { get; set; }

    public Warehouse Warehouse { get; set; } = null!;
    public ICollection<InventoryAuditItem> Items { get; set; } = new List<InventoryAuditItem>();
}
