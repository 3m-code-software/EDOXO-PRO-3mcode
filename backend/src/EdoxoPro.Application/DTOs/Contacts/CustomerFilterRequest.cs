using EdoxoPro.Application.Common;

namespace EdoxoPro.Application.DTOs.Contacts;

public class CustomerFilterRequest : FilterRequest
{
    public int? GroupId { get; set; }
}
