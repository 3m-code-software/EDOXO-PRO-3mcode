using EdoxoPro.Application.Common;
using EdoxoPro.Application.DTOs.Contacts;
using EdoxoPro.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EdoxoPro.Api.Controllers;

[ApiController]
[Route("api/delegates")]
[Authorize]
public class DelegatesController : ControllerBase
{
    private readonly IDelegateService _delegateService;

    public DelegatesController(IDelegateService delegateService)
    {
        _delegateService = delegateService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] DelegateFilterRequest request)
    {
        var result = await _delegateService.GetAllAsync(request);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _delegateService.GetByIdAsync(id);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateDelegateDto request)
    {
        var result = await _delegateService.CreateAsync(request);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateDelegateDto request)
    {
        var result = await _delegateService.UpdateAsync(id, request);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _delegateService.DeleteAsync(id);
        return result.Success ? Ok(result) : NotFound(result);
    }
}
