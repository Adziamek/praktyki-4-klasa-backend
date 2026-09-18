using Microsoft.AspNetCore.Mvc;
using Warehouse.Application.Interfaces;
using Warehouse.Domain.Entities;

namespace Warehouse.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BrandsController : ControllerBase
{
    private readonly IBrandService _brandService;

    public BrandsController(IBrandService brandService)
    {
        _brandService = brandService;
    }

    [HttpGet]
    [EndpointSummary("Get all brands")]
    public async Task<ActionResult<IEnumerable<Brand>>> GetAll()
    {
        return Ok(await _brandService.GetAllAsync());
    }

    [HttpGet("{id}")]
    [EndpointSummary("Get brand by id")]
    public async Task<ActionResult<Brand>> GetById(int id)
    {
        var brand = await _brandService.GetByIdAsync(id);
        if (brand is null) return NotFound();
        return Ok(brand);
    }

    [HttpPost]
    [EndpointSummary("Creates new brand")]
    public async Task<ActionResult<Brand>> Create(Brand brand)
    {
        var created = await _brandService.CreateAsync(brand);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    [EndpointSummary("Updates existing brand")]
    public async Task<ActionResult<Brand>> Update(int id, Brand brand)
    {
        var updated = await _brandService.UpdateAsync(id, brand);
        if (updated is null) return NotFound();
        return Ok(updated);
    }

    [HttpDelete("{id}")]
    [EndpointSummary("Deletes existing brand")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _brandService.DeleteAsync(id);
        if (!deleted) return NotFound();
        return NoContent();
    }
}