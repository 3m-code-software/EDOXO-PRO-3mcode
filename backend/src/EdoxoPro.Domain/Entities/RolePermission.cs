namespace EdoxoPro.Domain.Entities;

public class RolePermission
{
    public int Id { get; set; }
    public int RoleId { get; set; }
    public string PermissionName { get; set; } = string.Empty;

    public Role Role { get; set; } = null!;
}
