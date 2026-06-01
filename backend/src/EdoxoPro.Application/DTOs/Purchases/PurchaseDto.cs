namespace EdoxoPro.Application.DTOs.Purchases;

public class PurchaseDto
{
    public int Id { get; set; }
    public string ReferenceNumber { get; set; } = string.Empty;
    public int SupplierId { get; set; }
    public string SupplierName { get; set; } = string.Empty;
    public int? BranchId { get; set; }
    public DateTime Date { get; set; }
    public decimal Subtotal { get; set; }
    public decimal Tax { get; set; }
    public decimal TaxRate { get; set; }
    public decimal Total { get; set; }
    public decimal PaidAmount { get; set; }
    public string Status { get; set; } = string.Empty;
    public int? PaymentPeriod { get; set; }
    public string? Notes { get; set; }
    public List<PurchaseItemDto> Items { get; set; } = new();
}
