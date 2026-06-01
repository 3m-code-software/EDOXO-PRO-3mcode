namespace EdoxoPro.Application.DTOs.Reports;
public class TopSellingProductDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string? SKU { get; set; }
    public double TotalQuantity { get; set; }
    public decimal TotalRevenue { get; set; }
    public int OrderCount { get; set; }
}
