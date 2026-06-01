namespace EdoxoPro.Application.DTOs.Products;

public class UpdateProductVariantDto
{
    public string AttributeName { get; set; } = string.Empty;
    public string AttributeValue { get; set; } = string.Empty;
    public string? Barcode { get; set; }
    public decimal Price { get; set; }
    public decimal Cost { get; set; }
    public double Stock { get; set; }
}
