namespace EdoxoPro.Application.DTOs.Purchases;

public class CreatePurchaseDto
{
    public int SupplierId { get; set; }
    public int? BranchId { get; set; }
    public DateTime? Date { get; set; }
    public decimal TaxRate { get; set; }
    public int? PaymentPeriod { get; set; }
    public string? Notes { get; set; }
    public List<CreatePurchaseItemDto> Items { get; set; } = new();
}
