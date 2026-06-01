using EdoxoPro.Application.DTOs.Dashboard;
using EdoxoPro.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EdoxoPro.Api.Controllers;

[ApiController]
[Route("api/dashboard")]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboardService;

    public DashboardController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary()
    {
        var result = await _dashboardService.GetSummaryAsync();
        return Ok(result);
    }

    [HttpGet("sales-chart")]
    public async Task<IActionResult> GetSalesChart([FromQuery] int days = 30)
    {
        var result = await _dashboardService.GetSalesChartAsync(days);
        return Ok(result);
    }

    [HttpGet("annual-chart")]
    public async Task<IActionResult> GetAnnualChart([FromQuery] int year = 0)
    {
        if (year == 0) year = DateTime.UtcNow.Year;
        var result = await _dashboardService.GetAnnualChartAsync(year);
        return Ok(result);
    }

    [HttpGet("recent-orders")]
    public async Task<IActionResult> GetRecentOrders([FromQuery] int count = 10)
    {
        var result = await _dashboardService.GetRecentOrdersAsync(count);
        return Ok(result);
    }

    [HttpGet("pending-shipments")]
    public async Task<IActionResult> GetPendingShipments()
    {
        var result = await _dashboardService.GetPendingShipmentsAsync();
        return Ok(result);
    }

    [HttpGet("inventory-alerts")]
    public async Task<IActionResult> GetInventoryAlerts()
    {
        var result = await _dashboardService.GetInventoryAlertsAsync();
        return Ok(result);
    }

    [HttpGet("payment-dues")]
    public async Task<IActionResult> GetPaymentDues()
    {
        var result = await _dashboardService.GetPaymentDuesAsync();
        return Ok(result);
    }
}
