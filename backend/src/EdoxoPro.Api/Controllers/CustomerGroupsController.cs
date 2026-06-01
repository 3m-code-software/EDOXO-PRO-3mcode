using EdoxoPro.Application.Common;
using EdoxoPro.Application.DTOs.Contacts;
using EdoxoPro.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EdoxoPro.Api.Controllers;

[ApiController]
[Route("api/customer-groups")]
[Authorize]
public class CustomerGroupsController : ControllerBase
{
    private readonly ICustomerGroupService _customerGroupService;

    public CustomerGroupsController(ICustomerGroupService customerGroupService)
    {
        _customerGroupService = customerGroupService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _customerGroupService.GetAllAsync();
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _customerGroupService.GetByIdAsync(id);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCustomerGroupDto request)
    {
        var result = await _customerGroupService.CreateAsync(request);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateCustomerGroupDto request)
    {
        var result = await _customerGroupService.UpdateAsync(id, request);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _customerGroupService.DeleteAsync(id);
        return result.Success ? Ok(result) : NotFound(result);
    }
}
