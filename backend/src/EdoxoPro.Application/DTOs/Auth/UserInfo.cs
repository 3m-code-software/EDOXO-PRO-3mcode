namespace EdoxoPro.Application.DTOs.Auth;
public class UserInfo
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? FullNameAr { get; set; }
    public List<string> Roles { get; set; } = new();
}
