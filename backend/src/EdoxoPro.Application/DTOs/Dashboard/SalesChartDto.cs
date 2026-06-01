namespace EdoxoPro.Application.DTOs.Dashboard;
public class SalesChartDto
{
    public List<ChartDataPoint> SalesData { get; set; } = new();
    public List<ChartDataPoint> PurchasesData { get; set; } = new();
}
