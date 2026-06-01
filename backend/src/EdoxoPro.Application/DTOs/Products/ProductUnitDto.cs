namespace EdoxoPro.Application.DTOs.Products;

public class ProductUnitDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? ShortName { get; set; }
    public bool IsActive { get; set; }
}
