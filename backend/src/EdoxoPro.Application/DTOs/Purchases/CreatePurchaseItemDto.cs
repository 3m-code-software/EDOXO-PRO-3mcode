namespace EdoxoPro.Application.DTOs.Purchases;

public class CreatePurchaseItemDto
{
    public int ProductId { get; set; }
    public int? VariantId { get; set; }
    public double Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}
