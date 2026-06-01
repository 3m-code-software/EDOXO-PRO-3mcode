using EdoxoPro.Application.Common;

namespace EdoxoPro.Application.DTOs.Sales;

public class SaleFilterRequest : FilterRequest
{
    public string? Status { get; set; }
    public int? CustomerId { get; set; }
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
}
