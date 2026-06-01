using EdoxoPro.Application.Common;
using EdoxoPro.Application.DTOs.Purchases;
using EdoxoPro.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EdoxoPro.Api.Controllers;

[ApiController]
[Route("api/purchase-returns")]
[Authorize]
public class PurchaseReturnsController : ControllerBase
{
    private readonly IPurchaseReturnService _purchaseReturnService;

    public PurchaseReturnsController(IPurchaseReturnService purchaseReturnService)
    {
        _purchaseReturnService = purchaseReturnService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] FilterRequest request)
    {
        var result = await _purchaseReturnService.GetAllAsync(request);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _purchaseReturnService.GetByIdAsync(id);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePurchaseReturnDto request)
    {
        var result = await _purchaseReturnService.CreateAsync(request);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}
