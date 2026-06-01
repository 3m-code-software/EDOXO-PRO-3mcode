using EdoxoPro.Domain.Enums;

namespace EdoxoPro.Domain.Entities;

public class User : BaseEntity
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string PasswordHash { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? FullNameAr { get; set; }
    public bool IsActive { get; set; } = true;
    public int? BranchId { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public Gender? Gender { get; set; }
    public string? AvatarUrl { get; set; }

    public Branch? Branch { get; set; }
    public ICollection<UserRole> Roles { get; set; } = new List<UserRole>();
}
