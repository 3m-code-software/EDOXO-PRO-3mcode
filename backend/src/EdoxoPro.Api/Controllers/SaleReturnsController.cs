using EdoxoPro.Application.Common;
using EdoxoPro.Application.DTOs.Sales;
using EdoxoPro.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EdoxoPro.Api.Controllers;

[ApiController]
[Route("api/sales-returns")]
[Authorize]
public class SaleReturnsController : ControllerBase
{
    private readonly ISaleReturnService _saleReturnService;

    public SaleReturnsController(ISaleReturnService saleReturnService)
    {
        _saleReturnService = saleReturnService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] FilterRequest request)
    {
        var result = await _saleReturnService.GetAllAsync(request);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _saleReturnService.GetByIdAsync(id);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateSaleReturnDto request)
    {
        var result = await _saleReturnService.CreateAsync(request);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}
