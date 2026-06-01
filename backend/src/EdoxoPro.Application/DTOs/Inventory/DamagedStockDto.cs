namespace EdoxoPro.Application.DTOs.Inventory;

public class DamagedStockDto
{
    public int Id { get; set; }
    public string ReferenceNumber { get; set; } = string.Empty;
    public int WarehouseId { get; set; }
    public string WarehouseName { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string? Reason { get; set; }
    public decimal Total { get; set; }
    public string? Notes { get; set; }
    public List<DamagedStockItemDto> Items { get; set; } = new();
}

public class DamagedStockItemDto
{
    public int Id { get; set; }
    public int DamagedStockId { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public double Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Total { get; set; }
}
