namespace EdoxoPro.Application.DTOs.Users;

public class UpdateUserDto
{
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? FullName { get; set; }
    public bool? IsActive { get; set; }
    public List<int> RoleIds { get; set; } = new();
}
