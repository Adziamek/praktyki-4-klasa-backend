using Microsoft.AspNetCore.Mvc;
using Warehouse.Application.Interfaces;
using Warehouse.Domain.Entities;

namespace Warehouse.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoriesController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet]
    [EndpointSummary("Get all categories")]
    public async Task<ActionResult<IEnumerable<Category>>> GetAll()
    {
        return Ok(await _categoryService.GetAllAsync());
    }

    [HttpGet("{id}")]
    [EndpointSummary("Get category by id")]
    public async Task<ActionResult<Category>> GetById(int id)
    {
        var category = await _categoryService.GetByIdAsync(id);
        if (category is null) return NotFound();
        return Ok(category);
    }

    [HttpPost]
    [EndpointSummary("Creates new category")]

    public async Task<ActionResult<Category>> Create(Category category)
    {
        var created = await _categoryService.CreateAsync(category);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    [EndpointSummary("Updates existing category")]

    public async Task<ActionResult<Category>> Update(int id, Category category)
    {
        var updated = await _categoryService.UpdateAsync(id, category);
        if (updated is null) return NotFound();
        return Ok(updated);
    }

    [HttpDelete("{id}")]
    [EndpointSummary("Deletes existing category")]

    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _categoryService.DeleteAsync(id);
        if (!deleted) return NotFound();
        return NoContent();
    }
}