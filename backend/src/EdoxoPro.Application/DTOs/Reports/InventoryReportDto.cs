namespace EdoxoPro.Application.DTOs.Reports;
public class InventoryReportDto
{
    public int TotalProducts { get; set; }
    public int LowStockProducts { get; set; }
    public int OutOfStockProducts { get; set; }
    public decimal TotalStockValue { get; set; }
    public decimal AverageCostPrice { get; set; }
    public DateTime AsOfDate { get; set; }
}
