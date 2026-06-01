using EdoxoPro.Domain.Enums;

namespace EdoxoPro.Domain.Entities;

public class Sale : BaseEntity
{
    public string InvoiceNumber { get; set; } = string.Empty;
    public int CustomerId { get; set; }
    public int? DelegateId { get; set; }
    public int? BranchId { get; set; }
    public DateTime Date { get; set; } = DateTime.UtcNow;
    public decimal Subtotal { get; set; }
    public decimal Discount { get; set; }
    public string DiscountType { get; set; } = "Fixed";
    public decimal Tax { get; set; }
    public decimal TaxRate { get; set; }
    public decimal Total { get; set; }
    public decimal PaidAmount { get; set; }
    public SaleStatus Status { get; set; } = SaleStatus.Draft;
    public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Unpaid;
    public ShippingStatus ShippingStatus { get; set; } = ShippingStatus.Pending;
    public string? Notes { get; set; }

    public Customer Customer { get; set; } = null!;
    public Delegate? Delegate { get; set; }
    public ICollection<SaleItem> Items { get; set; } = new List<SaleItem>();
}
