using EdoxoPro.Application.Common;

namespace EdoxoPro.Application.DTOs.Products;

public class ProductFilterRequest : FilterRequest
{
    public int? CategoryId { get; set; }
    public int? BrandId { get; set; }
    public bool? IsActive { get; set; }
    public bool? LowStock { get; set; }
}
