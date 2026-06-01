using EdoxoPro.Application.Common;

namespace EdoxoPro.Application.DTOs.Users;

public class UserFilterRequest : FilterRequest
{
    public bool? IsActive { get; set; }
    public int? RoleId { get; set; }
}
