using Microsoft.AspNetCore.Identity;

namespace EdoxoPro.Infrastructure.Identity;

public class AppIdentityUser : IdentityUser<int>
{
    public string FullName { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public int? BranchId { get; set; }
}
