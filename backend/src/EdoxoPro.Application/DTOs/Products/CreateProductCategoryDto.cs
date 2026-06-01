namespace EdoxoPro.Application.DTOs.Products;

public class CreateProductCategoryDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? ParentId { get; set; }
}
