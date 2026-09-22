using Microsoft.AspNetCore.Mvc;
using Warehouse.Application.DTO.Category;
using Warehouse.Application.Interfaces;

namespace Warehouse.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categoryService;
    private readonly ILogger<CategoriesController> _logger;

    public CategoriesController(
        ICategoryService categoryService,
        ILogger<CategoriesController> logger)
    {
        _categoryService = categoryService;
        _logger = logger;
    }

    // GET: api/Categories
    [HttpGet]
    [EndpointSummary("Category list")]
    [ProducesResponseType(typeof(IReadOnlyList<CategoryResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<CategoryResponseDto>>> GetCategories()
    {
        var Categories = await _categoryService.GetAllAsync();

        return Ok(Categories);
    }

    // GET: api/Categories/{id}
    [HttpGet("{id:int}")]
    [EndpointSummary("Returns category by id")]
    [ProducesResponseType(typeof(CategoryResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CategoryResponseDto>> GetCategoryById(int id)
    {
        var Category = await _categoryService.GetByIdAsync(id);

        if (Category is null)
            return NotFound();

        return Ok(Category);
    }

    // POST: api/Categories
    [HttpPost]
    [EndpointSummary("Creates new category")]
    [ProducesResponseType(typeof(CategoryResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CategoryResponseDto>> AddCategory(CategoryDto dto)
    {
        _logger.LogDebug(
            "Adding category: {Name}",
            dto.Name);

        var result = await _categoryService.AddAsync(dto);

        if (!result.Success)
        {
            return Problem(
                title: result.Error,
                detail: result.Error,
                statusCode: result.StatusCode);
        }

        var Category = result.Category!;

        return CreatedAtAction(
            nameof(GetCategoryById),
            new { id = Category.Id },
            Category);
    }

    // PUT: api/Categories/{id}
    [HttpPut("{id:int}")]
    [EndpointSummary("Updates existing Category")]
    [ProducesResponseType(typeof(CategoryResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CategoryResponseDto>> UpdateCategory(
        int id,
        CategoryDto dto)
    {
        var result = await _categoryService.UpdateAsync(id, dto);

        if (!result.Success)
        {
            return Problem(
                title: result.Error,
                detail: result.Error,
                statusCode: result.StatusCode);
        }

        return Ok(result.Category);
    }

    // DELETE: api/Categorys/{id}
    [HttpDelete("{id:int}")]
    [EndpointSummary("Deletes existing Category")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        var deleted = await _categoryService.DeleteAsync(id);

        if (!deleted)
            return NotFound();

        return NoContent();
    }
}