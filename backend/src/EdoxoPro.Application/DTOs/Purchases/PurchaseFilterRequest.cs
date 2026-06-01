using EdoxoPro.Application.Common;

namespace EdoxoPro.Application.DTOs.Purchases;

public class PurchaseFilterRequest : FilterRequest
{
    public string? Status { get; set; }
    public int? SupplierId { get; set; }
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
}
