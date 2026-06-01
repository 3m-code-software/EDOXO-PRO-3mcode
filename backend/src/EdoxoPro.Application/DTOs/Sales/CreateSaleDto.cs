namespace EdoxoPro.Application.DTOs.Sales;

public class CreateSaleDto
{
    public int CustomerId { get; set; }
    public int? DelegateId { get; set; }
    public int? BranchId { get; set; }
    public DateTime? Date { get; set; }
    public decimal Discount { get; set; }
    public string DiscountType { get; set; } = "Fixed";
    public decimal TaxRate { get; set; }
    public string? Notes { get; set; }
    public List<CreateSaleItemDto> Items { get; set; } = new();
}
