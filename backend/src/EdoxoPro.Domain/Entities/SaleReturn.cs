namespace EdoxoPro.Domain.Entities;

public class SaleReturn : BaseEntity
{
    public string ReturnNumber { get; set; } = string.Empty;
    public int SaleId { get; set; }
    public DateTime Date { get; set; } = DateTime.UtcNow;
    public string? Reason { get; set; }
    public decimal Total { get; set; }

    public Sale Sale { get; set; } = null!;
    public ICollection<SaleReturnItem> Items { get; set; } = new List<SaleReturnItem>();
}
