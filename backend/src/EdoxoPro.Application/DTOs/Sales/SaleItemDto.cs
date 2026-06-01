namespace EdoxoPro.Application.DTOs.Sales;

public class SaleItemDto
{
    public int Id { get; set; }
    public int SaleId { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string SKU { get; set; } = string.Empty;
    public int? VariantId { get; set; }
    public double Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Discount { get; set; }
    public string DiscountType { get; set; } = "Fixed";
    public decimal Total { get; set; }
}
