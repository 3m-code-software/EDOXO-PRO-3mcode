using EdoxoPro.Application.Common;
using EdoxoPro.Application.DTOs.Contacts;
using EdoxoPro.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EdoxoPro.Api.Controllers;

[ApiController]
[Route("api/suppliers")]
[Authorize]
public class SuppliersController : ControllerBase
{
    private readonly ISupplierService _supplierService;

    public SuppliersController(ISupplierService supplierService)
    {
        _supplierService = supplierService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] SupplierFilterRequest request)
    {
        var result = await _supplierService.GetAllAsync(request);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _supplierService.GetByIdAsync(id);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateSupplierDto dto)
    {
        var result = await _supplierService.CreateAsync(dto);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateSupplierDto dto)
    {
        var result = await _supplierService.UpdateAsync(id, dto);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _supplierService.DeleteAsync(id);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpGet("export")]
    public async Task<IActionResult> Export([FromQuery] FilterRequest request)
    {
        var result = await _supplierService.ExportAsync(request);
        return result.Success ? File(result.Data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "suppliers.xlsx") : BadRequest(result);
    }
}
