using EdoxoPro.Application.Common;
using EdoxoPro.Application.DTOs.Sales;
using EdoxoPro.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EdoxoPro.Api.Controllers;

[ApiController]
[Route("api/sales")]
[Authorize]
public class SalesController : ControllerBase
{
    private readonly ISaleService _saleService;

    public SalesController(ISaleService saleService)
    {
        _saleService = saleService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] SaleFilterRequest request)
    {
        var result = await _saleService.GetAllAsync(request);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _saleService.GetByIdAsync(id);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateSaleDto dto)
    {
        var result = await _saleService.CreateAsync(dto);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateSaleDto dto)
    {
        var result = await _saleService.UpdateAsync(id, dto);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _saleService.DeleteAsync(id);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpPost("{id}/approve")]
    public async Task<IActionResult> Approve(int id)
    {
        var result = await _saleService.ApproveAsync(id);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpPost("{id}/pay")]
    public async Task<IActionResult> Pay(int id, [FromBody] PaySaleDto dto)
    {
        var result = await _saleService.PayAsync(id, dto);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("drafts")]
    public async Task<IActionResult> GetDrafts()
    {
        var result = await _saleService.GetDraftsAsync();
        return Ok(result);
    }

    [HttpGet("quotes")]
    public async Task<IActionResult> GetQuotes()
    {
        var result = await _saleService.GetQuotesAsync();
        return Ok(result);
    }

    [HttpGet("export")]
    public async Task<IActionResult> Export([FromQuery] SaleFilterRequest request)
    {
        var result = await _saleService.ExportAsync(request);
        return result.Success ? File(result.Data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "sales.xlsx") : BadRequest(result);
    }
}
