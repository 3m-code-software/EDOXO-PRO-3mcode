using EdoxoPro.Application.DTOs.Settings;
using EdoxoPro.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EdoxoPro.Api.Controllers;

[ApiController]
[Route("api/settings")]
[Authorize]
public class SettingsController : ControllerBase
{
    private readonly ICompanySettingService _companySettingService;
    private readonly IInvoiceSettingService _invoiceSettingService;
    private readonly IBarcodeSettingService _barcodeSettingService;

    public SettingsController(
        ICompanySettingService companySettingService,
        IInvoiceSettingService invoiceSettingService,
        IBarcodeSettingService barcodeSettingService)
    {
        _companySettingService = companySettingService;
        _invoiceSettingService = invoiceSettingService;
        _barcodeSettingService = barcodeSettingService;
    }

    [HttpGet("company")]
    public async Task<IActionResult> GetCompanySettings()
    {
        var result = await _companySettingService.GetAsync();
        return Ok(result);
    }

    [HttpPut("company")]
    public async Task<IActionResult> UpdateCompanySettings([FromBody] UpdateCompanySettingDto dto)
    {
        var result = await _companySettingService.UpdateAsync(dto);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("invoices")]
    public async Task<IActionResult> GetInvoiceSettings()
    {
        var result = await _invoiceSettingService.GetAsync();
        return Ok(result);
    }

    [HttpPut("invoices")]
    public async Task<IActionResult> UpdateInvoiceSettings([FromBody] UpdateInvoiceSettingDto dto)
    {
        var result = await _invoiceSettingService.UpdateAsync(dto);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("barcode")]
    public async Task<IActionResult> GetBarcodeSettings()
    {
        var result = await _barcodeSettingService.GetAsync();
        return Ok(result);
    }

    [HttpPut("barcode")]
    public async Task<IActionResult> UpdateBarcodeSettings([FromBody] UpdateBarcodeSettingDto dto)
    {
        var result = await _barcodeSettingService.UpdateAsync(dto);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}
