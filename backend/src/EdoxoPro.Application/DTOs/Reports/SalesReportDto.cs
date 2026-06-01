namespace EdoxoPro.Application.DTOs.Reports;
public class SalesReportDto
{
    public decimal TotalSales { get; set; }
    public int TotalOrders { get; set; }
    public decimal TotalDiscounts { get; set; }
    public decimal TotalTax { get; set; }
    public decimal TotalShipping { get; set; }
    public int TotalItems { get; set; }
    public int ConfirmedOrders { get; set; }
    public int DraftOrders { get; set; }
    public int CancelledOrders { get; set; }
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
}
