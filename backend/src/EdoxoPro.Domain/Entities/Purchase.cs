using EdoxoPro.Domain.Enums;

namespace EdoxoPro.Domain.Entities;

public class Purchase : BaseEntity
{
    public string ReferenceNumber { get; set; } = string.Empty;
    public int SupplierId { get; set; }
    public int? BranchId { get; set; }
    public DateTime Date { get; set; } = DateTime.UtcNow;
    public decimal Subtotal { get; set; }
    public decimal Tax { get; set; }
    public decimal TaxRate { get; set; }
    public decimal Total { get; set; }
    public decimal PaidAmount { get; set; }
    public PurchaseStatus Status { get; set; } = PurchaseStatus.Draft;
    public int? PaymentPeriod { get; set; }
    public string? Notes { get; set; }

    public Supplier Supplier { get; set; } = null!;
    public ICollection<PurchaseItem> Items { get; set; } = new List<PurchaseItem>();
}
