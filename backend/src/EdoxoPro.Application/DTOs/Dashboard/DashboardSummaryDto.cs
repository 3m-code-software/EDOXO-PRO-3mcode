namespace EdoxoPro.Application.DTOs.Dashboard;
public class DashboardSummaryDto
{
    public decimal TotalSales { get; set; }
    public int TotalSalesCount { get; set; }
    public decimal TotalPurchases { get; set; }
    public int TotalPurchasesCount { get; set; }
    public decimal TotalExpenses { get; set; }
    public int TotalExpensesCount { get; set; }
    public int TotalProducts { get; set; }
    public int TotalCustomers { get; set; }
    public int TotalSuppliers { get; set; }
    public decimal NetProfit { get; set; }
    public int SalesCount { get; set; }
    public int PurchaseCount { get; set; }
    public int CustomerCount { get; set; }
    public int SupplierCount { get; set; }
    public int ProductCount { get; set; }
    public int LowStockCount { get; set; }
}
