using EdoxoPro.Application.DTOs.Shared;
namespace EdoxoPro.Application.DTOs.Reports;
public class ReportRequest : DateRangeFilter
{
    public int? BranchId { get; set; }
    public int? ProductId { get; set; }
    public int? CategoryId { get; set; }
}
