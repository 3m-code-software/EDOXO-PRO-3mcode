using EdoxoPro.Application.DTOs.Shared;
namespace EdoxoPro.Application.DTOs.Purchases;
public class UpdatePurchaseDto
{
    public DateTime? PurchaseDate { get; set; }
    public int? SupplierId { get; set; }
    public string? ReferenceNumber { get; set; }
    public string? Notes { get; set; }
    public List<CreatePurchaseItemDto> Items { get; set; } = new();
    public string? Status { get; set; }
}
