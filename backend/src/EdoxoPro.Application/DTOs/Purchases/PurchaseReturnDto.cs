namespace EdoxoPro.Application.DTOs.Purchases;

public class PurchaseReturnDto
{
    public int Id { get; set; }
    public string ReturnNumber { get; set; } = string.Empty;
    public int PurchaseId { get; set; }
    public string ReferenceNumber { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string? Reason { get; set; }
    public decimal Total { get; set; }
    public List<PurchaseReturnItemDto> Items { get; set; } = new();
}

public class PurchaseReturnItemDto
{
    public int Id { get; set; }
    public int ReturnId { get; set; }
    public int PurchaseItemId { get; set; }
    public double Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}
