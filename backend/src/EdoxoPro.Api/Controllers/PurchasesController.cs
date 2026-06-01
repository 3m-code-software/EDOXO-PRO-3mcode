using EdoxoPro.Application.Common;
using EdoxoPro.Application.DTOs.Purchases;
using EdoxoPro.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EdoxoPro.Api.Controllers;

[ApiController]
[Route("api/purchases")]
[Authorize]
public class PurchasesController : ControllerBase
{
    private readonly IPurchaseService _purchaseService;

    public PurchasesController(IPurchaseService purchaseService)
    {
        _purchaseService = purchaseService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] PurchaseFilterRequest request)
    {
        var result = await _purchaseService.GetAllAsync(request);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _purchaseService.GetByIdAsync(id);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePurchaseDto dto)
    {
        var result = await _purchaseService.CreateAsync(dto);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdatePurchaseDto dto)
    {
        var result = await _purchaseService.UpdateAsync(id, dto);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _purchaseService.DeleteAsync(id);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpPost("{id}/receive")]
    public async Task<IActionResult> Receive(int id)
    {
        var result = await _purchaseService.ReceiveAsync(id);
        return result.Success ? Ok(result) : NotFound(result);
    }
}
