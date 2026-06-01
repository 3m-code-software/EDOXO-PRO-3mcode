using EdoxoPro.Application.Common;
using EdoxoPro.Application.DTOs.Reports;

namespace EdoxoPro.Application.Interfaces;
public interface IReportService
{
    Task<ApiResponse<ProfitLossDto>> GetProfitLossAsync(ReportRequest request);
    Task<ApiResponse<SalesReportDto>> GetSalesReportAsync(ReportRequest request);
    Task<ApiResponse<InventoryReportDto>> GetInventoryReportAsync(ReportRequest request);
    Task<ApiResponse<List<TopSellingProductDto>>> GetTopSellingAsync(ReportRequest request);
}
