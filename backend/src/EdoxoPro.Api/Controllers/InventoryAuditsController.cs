using EdoxoPro.Application.Common;
using EdoxoPro.Application.DTOs.Inventory;
using EdoxoPro.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EdoxoPro.Api.Controllers;

[ApiController]
[Route("api/inventory-audits")]
[Authorize]
public class InventoryAuditsController : ControllerBase
{
    private readonly IInventoryAuditService _auditService;

    public InventoryAuditsController(IInventoryAuditService auditService)
    {
        _auditService = auditService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] FilterRequest request)
    {
        var result = await _auditService.GetAllAsync(request);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _auditService.GetByIdAsync(id);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateInventoryAuditDto dto)
    {
        var result = await _auditService.CreateAsync(dto);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPut("{id}/start")]
    public async Task<IActionResult> Start(int id)
    {
        var result = await _auditService.StartAsync(id);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPut("{id}/complete")]
    public async Task<IActionResult> Complete(int id)
    {
        var result = await _auditService.CompleteAsync(id);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}
