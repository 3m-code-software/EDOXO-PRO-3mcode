namespace EdoxoPro.Application.DTOs.Inventory;

public class CreateStockTransferDto
{
    public int FromWarehouseId { get; set; }
    public int ToWarehouseId { get; set; }
    public DateTime Date { get; set; }
    public string? Notes { get; set; }
    public List<CreateStockTransferItemDto> Items { get; set; } = new();
}

public class CreateStockTransferItemDto
{
    public int ProductId { get; set; }
    public double Quantity { get; set; }
}
