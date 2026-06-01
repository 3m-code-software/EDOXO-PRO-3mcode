namespace EdoxoPro.Domain.Entities;

public class ProductUnit : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? ShortName { get; set; }
    public bool IsActive { get; set; } = true;
}
