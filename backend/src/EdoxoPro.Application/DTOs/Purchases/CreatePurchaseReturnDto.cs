namespace EdoxoPro.Application.DTOs.Purchases;

public class CreatePurchaseReturnDto
{
    public int PurchaseId { get; set; }
    public DateTime Date { get; set; }
    public string? Reason { get; set; }
    public List<CreatePurchaseReturnItemDto> Items { get; set; } = new();
}

public class CreatePurchaseReturnItemDto
{
    public int PurchaseItemId { get; set; }
    public double Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}
