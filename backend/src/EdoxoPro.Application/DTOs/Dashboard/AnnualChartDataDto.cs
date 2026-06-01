namespace EdoxoPro.Application.DTOs.Dashboard;

public class AnnualChartDataDto
{
    public string Month { get; set; } = string.Empty;
    public decimal SalesAmount { get; set; }
    public decimal PurchaseAmount { get; set; }
    public decimal Profit { get; set; }
}
