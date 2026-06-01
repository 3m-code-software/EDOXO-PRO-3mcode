namespace EdoxoPro.Application.DTOs.Inventory;

public class CreateInventoryAuditDto
{
    public int WarehouseId { get; set; }
    public DateTime Date { get; set; }
    public string? Notes { get; set; }
    public List<CreateInventoryAuditItemDto> Items { get; set; } = new();
}

public class CreateInventoryAuditItemDto
{
    public int ProductId { get; set; }
    public double SystemQuantity { get; set; }
    public double ActualQuantity { get; set; }
    public decimal UnitPrice { get; set; }
}
