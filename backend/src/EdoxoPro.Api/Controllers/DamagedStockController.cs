using EdoxoPro.Application.Common;
using EdoxoPro.Application.DTOs.Inventory;
using EdoxoPro.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EdoxoPro.Api.Controllers;

[ApiController]
[Route("api/damaged-stock")]
[Authorize]
public class DamagedStockController : ControllerBase
{
    private readonly IDamagedStockService _damagedStockService;

    public DamagedStockController(IDamagedStockService damagedStockService)
    {
        _damagedStockService = damagedStockService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] FilterRequest request)
    {
        var result = await _damagedStockService.GetAllAsync(request);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _damagedStockService.GetByIdAsync(id);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateDamagedStockDto dto)
    {
        var result = await _damagedStockService.CreateAsync(dto);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}
