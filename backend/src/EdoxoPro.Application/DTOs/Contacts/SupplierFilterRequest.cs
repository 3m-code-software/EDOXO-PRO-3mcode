using EdoxoPro.Application.Common;

namespace EdoxoPro.Application.DTOs.Contacts;

public class SupplierFilterRequest : FilterRequest
{
    public bool? IsActive { get; set; }
    public int? PaymentPeriod { get; set; }
}
