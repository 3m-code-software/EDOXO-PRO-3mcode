namespace EdoxoPro.Application.DTOs.Inventory;

public class StockTransferDto
{
    public int Id { get; set; }
    public string TransferNumber { get; set; } = string.Empty;
    public int FromWarehouseId { get; set; }
    public string FromWarehouseName { get; set; } = string.Empty;
    public int ToWarehouseId { get; set; }
    public string ToWarehouseName { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public List<StockTransferItemDto> Items { get; set; } = new();
}

public class StockTransferItemDto
{
    public int Id { get; set; }
    public int TransferId { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public double Quantity { get; set; }
}
