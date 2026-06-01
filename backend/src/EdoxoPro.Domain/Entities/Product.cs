namespace EdoxoPro.Domain.Entities;

public class Product : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? NameAr { get; set; }
    public string SKU { get; set; } = string.Empty;
    public string? Barcode { get; set; }
    public string? Description { get; set; }
    public int? CategoryId { get; set; }
    public int? BrandId { get; set; }
    public int? UnitId { get; set; }
    public decimal CostPrice { get; set; }
    public decimal SalePrice { get; set; }
    public decimal WholesalePrice { get; set; }
    public double MinStock { get; set; }
    public double CurrentStock { get; set; }
    public bool IsActive { get; set; } = true;
    public bool HasVariants { get; set; }
    public string? ImageUrl { get; set; }

    public ProductCategory? Category { get; set; }
    public ProductBrand? Brand { get; set; }
    public ProductUnit? Unit { get; set; }
    public ICollection<ProductVariant> Variants { get; set; } = new List<ProductVariant>();
    public ICollection<SaleItem> SaleItems { get; set; } = new List<SaleItem>();
    public ICollection<PurchaseItem> PurchaseItems { get; set; } = new List<PurchaseItem>();
    public ICollection<StockMovement> StockMovements { get; set; } = new List<StockMovement>();
}
