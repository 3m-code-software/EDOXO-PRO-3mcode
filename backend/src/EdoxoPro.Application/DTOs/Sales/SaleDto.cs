namespace EdoxoPro.Application.DTOs.Sales;

public class SaleDto
{
    public int Id { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public int CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public int? DelegateId { get; set; }
    public string? DelegateName { get; set; }
    public int? BranchId { get; set; }
    public DateTime Date { get; set; }
    public decimal Subtotal { get; set; }
    public decimal Discount { get; set; }
    public string DiscountType { get; set; } = "Fixed";
    public decimal Tax { get; set; }
    public decimal TaxRate { get; set; }
    public decimal Total { get; set; }
    public decimal PaidAmount { get; set; }
    public string Status { get; set; } = string.Empty;
    public string PaymentStatus { get; set; } = string.Empty;
    public string ShippingStatus { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public List<SaleItemDto> Items { get; set; } = new();
}
