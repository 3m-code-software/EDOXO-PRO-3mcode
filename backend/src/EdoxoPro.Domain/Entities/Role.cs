namespace EdoxoPro.Domain.Entities;

public class Role
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? NameAr { get; set; }
    public string? Description { get; set; }
    public bool IsSystem { get; set; }

    public ICollection<UserRole> Users { get; set; } = new List<UserRole>();
}
