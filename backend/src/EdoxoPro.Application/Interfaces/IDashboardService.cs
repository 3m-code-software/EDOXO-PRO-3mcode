using EdoxoPro.Application.Common;
using EdoxoPro.Application.DTOs.Dashboard;

namespace EdoxoPro.Application.Interfaces;
public interface IDashboardService
{
    Task<ApiResponse<DashboardSummaryDto>> GetSummaryAsync();
    Task<ApiResponse<SalesChartDto>> GetSalesChartAsync(int days);
    Task<ApiResponse<List<AnnualChartDataDto>>> GetAnnualChartAsync(int year);
    Task<ApiResponse<List<RecentOrderDto>>> GetRecentOrdersAsync(int count);
    Task<ApiResponse<List<PendingShipmentDto>>> GetPendingShipmentsAsync();
    Task<ApiResponse<List<InventoryAlertDto>>> GetInventoryAlertsAsync();
    Task<ApiResponse<List<PaymentDueDto>>> GetPaymentDuesAsync();
}
