namespace EdoxoPro.Application.DTOs.Products;

public class ProductDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? NameAr { get; set; }
    public string SKU { get; set; } = string.Empty;
    public string? Barcode { get; set; }
    public string? Description { get; set; }
    public int? CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public int? BrandId { get; set; }
    public string? BrandName { get; set; }
    public int? UnitId { get; set; }
    public string? UnitName { get; set; }
    public decimal CostPrice { get; set; }
    public decimal SalePrice { get; set; }
    public decimal WholesalePrice { get; set; }
    public double MinStock { get; set; }
    public double CurrentStock { get; set; }
    public bool IsActive { get; set; }
    public bool HasVariants { get; set; }
    public string? ImageUrl { get; set; }
}
