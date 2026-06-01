using EdoxoPro.Application.Common;

namespace EdoxoPro.Application.DTOs.Contacts;

public class DelegateFilterRequest : FilterRequest
{
    public bool? IsActive { get; set; }
}
