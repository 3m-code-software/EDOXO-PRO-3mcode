namespace EdoxoPro.Application.DTOs.Roles;

public class UpdateRoleDto
{
    public string Name { get; set; } = string.Empty;
    public string? NameAr { get; set; }
    public string? Description { get; set; }
    public List<string> Permissions { get; set; } = new();
}
