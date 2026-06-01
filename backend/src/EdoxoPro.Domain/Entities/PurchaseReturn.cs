namespace EdoxoPro.Domain.Entities;

public class PurchaseReturn : BaseEntity
{
    public string ReturnNumber { get; set; } = string.Empty;
    public int PurchaseId { get; set; }
    public DateTime Date { get; set; } = DateTime.UtcNow;
    public string? Reason { get; set; }
    public decimal Total { get; set; }

    public Purchase Purchase { get; set; } = null!;
    public ICollection<PurchaseReturnItem> Items { get; set; } = new List<PurchaseReturnItem>();
}
