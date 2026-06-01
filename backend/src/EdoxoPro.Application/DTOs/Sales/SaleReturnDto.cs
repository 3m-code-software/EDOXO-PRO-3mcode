namespace EdoxoPro.Application.DTOs.Sales;

public class SaleReturnDto
{
    public int Id { get; set; }
    public string ReturnNumber { get; set; } = string.Empty;
    public int SaleId { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string? Reason { get; set; }
    public decimal Total { get; set; }
    public List<SaleReturnItemDto> Items { get; set; } = new();
}

public class SaleReturnItemDto
{
    public int Id { get; set; }
    public int ReturnId { get; set; }
    public int SaleItemId { get; set; }
    public double Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Total { get; set; }
}
