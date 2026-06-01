namespace EdoxoPro.Application.DTOs.Sales;

public class CreateSaleItemDto
{
    public int ProductId { get; set; }
    public int? VariantId { get; set; }
    public double Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Discount { get; set; }
    public string DiscountType { get; set; } = "Fixed";
}
