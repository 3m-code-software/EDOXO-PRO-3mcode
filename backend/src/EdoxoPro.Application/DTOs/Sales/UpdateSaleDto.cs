namespace EdoxoPro.Application.DTOs.Sales;
public class UpdateSaleDto
{
    public DateTime? SaleDate { get; set; }
    public int? CustomerId { get; set; }
    public string? ReferenceNumber { get; set; }
    public string? Notes { get; set; }
    public string? Status { get; set; }
    public List<CreateSaleItemDto> Items { get; set; } = new();
}
