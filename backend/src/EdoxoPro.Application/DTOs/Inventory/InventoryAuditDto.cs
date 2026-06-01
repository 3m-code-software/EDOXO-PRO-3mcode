namespace EdoxoPro.Application.DTOs.Inventory;

public class InventoryAuditDto
{
    public int Id { get; set; }
    public string AuditNumber { get; set; } = string.Empty;
    public int WarehouseId { get; set; }
    public string WarehouseName { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public List<InventoryAuditItemDto> Items { get; set; } = new();
}

public class InventoryAuditItemDto
{
    public int Id { get; set; }
    public int AuditId { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public double SystemQuantity { get; set; }
    public double ActualQuantity { get; set; }
    public double Difference { get; set; }
    public decimal UnitPrice { get; set; }
}
