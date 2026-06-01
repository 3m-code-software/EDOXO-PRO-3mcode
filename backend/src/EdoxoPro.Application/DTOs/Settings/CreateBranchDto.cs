namespace EdoxoPro.Application.DTOs.Settings;
public class CreateBranchDto
{
    public string Name { get; set; } = string.Empty;
    public string? NameAr { get; set; }
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public bool IsActive { get; set; }
    public bool IsMainBranch { get; set; }
}
