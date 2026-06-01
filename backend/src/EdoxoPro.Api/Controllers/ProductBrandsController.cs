using EdoxoPro.Application.Common;
using EdoxoPro.Application.DTOs.Products;
using EdoxoPro.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EdoxoPro.Api.Controllers;

[ApiController]
[Route("api/brands")]
[Authorize]
public class ProductBrandsController : ControllerBase
{
    private readonly IProductBrandService _brandService;

    public ProductBrandsController(IProductBrandService brandService)
    {
        _brandService = brandService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _brandService.GetAllAsync();
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _brandService.GetByIdAsync(id);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProductBrandDto request)
    {
        var result = await _brandService.CreateAsync(request);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateProductBrandDto request)
    {
        var result = await _brandService.UpdateAsync(id, request);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _brandService.DeleteAsync(id);
        return result.Success ? Ok(result) : NotFound(result);
    }
}
