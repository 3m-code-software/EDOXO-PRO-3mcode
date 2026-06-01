using EdoxoPro.Application.DTOs.Reports;
using EdoxoPro.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EdoxoPro.Api.Controllers;

[ApiController]
[Route("api/reports")]
[Authorize]
public class ReportsController : ControllerBase
{
    private readonly IReportService _reportService;

    public ReportsController(IReportService reportService)
    {
        _reportService = reportService;
    }

    [HttpGet("profit-loss")]
    public async Task<IActionResult> GetProfitLoss([FromQuery] ReportRequest request)
    {
        var result = await _reportService.GetProfitLossAsync(request);
        return Ok(result);
    }

    [HttpGet("sales")]
    public async Task<IActionResult> GetSalesReport([FromQuery] ReportRequest request)
    {
        var result = await _reportService.GetSalesReportAsync(request);
        return Ok(result);
    }

    [HttpGet("inventory")]
    public async Task<IActionResult> GetInventoryReport([FromQuery] ReportRequest request)
    {
        var result = await _reportService.GetInventoryReportAsync(request);
        return Ok(result);
    }

    [HttpGet("top-selling")]
    public async Task<IActionResult> GetTopSelling([FromQuery] ReportRequest request)
    {
        var result = await _reportService.GetTopSellingAsync(request);
        return Ok(result);
    }
}
