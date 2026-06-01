using EdoxoPro.Application.Common;
using EdoxoPro.Application.DTOs.Checks;
using EdoxoPro.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EdoxoPro.Api.Controllers;

[ApiController]
[Route("api/checks")]
[Authorize]
public class ChecksController : ControllerBase
{
    private readonly ICheckService _checkService;

    public ChecksController(ICheckService checkService)
    {
        _checkService = checkService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] FilterRequest request)
    {
        var result = await _checkService.GetAllAsync(request);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _checkService.GetByIdAsync(id);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCheckDto dto)
    {
        var result = await _checkService.CreateAsync(dto);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateCheckDto dto)
    {
        var result = await _checkService.UpdateAsync(id, dto);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateCheckStatusDto dto)
    {
        var result = await _checkService.UpdateStatusAsync(id, dto);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}
