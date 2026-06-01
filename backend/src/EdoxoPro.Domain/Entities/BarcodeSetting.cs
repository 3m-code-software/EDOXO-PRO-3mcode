namespace EdoxoPro.Domain.Entities;

public class BarcodeSetting : BaseEntity
{
    public string? Format { get; set; }
    public string? Prefix { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public bool IsActive { get; set; } = true;
}
