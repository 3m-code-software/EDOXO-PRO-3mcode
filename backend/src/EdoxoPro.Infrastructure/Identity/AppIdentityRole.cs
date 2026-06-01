using Microsoft.AspNetCore.Identity;

namespace EdoxoPro.Infrastructure.Identity;

public class AppIdentityRole : IdentityRole<int>
{
    public AppIdentityRole() : base() { }
    public AppIdentityRole(string roleName) : base(roleName) { }
    public string? Description { get; set; }
    public bool IsSystem { get; set; }
}
