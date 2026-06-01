namespace EdoxoPro.Application.DTOs.Reports;
public class ProfitLossDto
{
    public decimal TotalRevenue { get; set; }
    public decimal TotalCogs { get; set; }
    public decimal GrossProfit => TotalRevenue - TotalCogs;
    public decimal TotalExpenses { get; set; }
    public decimal NetProfit => GrossProfit - TotalExpenses;
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
}
