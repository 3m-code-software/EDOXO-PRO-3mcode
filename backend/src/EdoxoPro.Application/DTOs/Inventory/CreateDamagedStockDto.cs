namespace EdoxoPro.Application.DTOs.Inventory;

public class CreateDamagedStockDto
{
    public int WarehouseId { get; set; }
    public DateTime Date { get; set; }
    public string? Reason { get; set; }
    public string? Notes { get; set; }
    public List<CreateDamagedStockItemDto> Items { get; set; } = new();
}

public class CreateDamagedStockItemDto
{
    public int ProductId { get; set; }
    public double Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}
