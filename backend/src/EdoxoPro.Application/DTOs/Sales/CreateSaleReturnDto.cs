namespace EdoxoPro.Application.DTOs.Sales;

public class CreateSaleReturnDto
{
    public int SaleId { get; set; }
    public DateTime Date { get; set; }
    public string? Reason { get; set; }
    public List<CreateSaleReturnItemDto> Items { get; set; } = new();
}

public class CreateSaleReturnItemDto
{
    public int SaleItemId { get; set; }
    public double Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}
