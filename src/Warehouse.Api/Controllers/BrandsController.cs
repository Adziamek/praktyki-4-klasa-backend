using Microsoft.AspNetCore.Mvc;
using Warehouse.Application.DTO.Brand;
using Warehouse.Application.Interfaces;

namespace Warehouse.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BrandsController : ControllerBase
{
    private readonly IBrandService _brandService;
    private readonly ILogger<BrandsController> _logger;

    public BrandsController(
        IBrandService brandService,
        ILogger<BrandsController> logger)
    {
        _brandService = brandService;
        _logger = logger;
    }

    // GET: api/brands
    [HttpGet]
    [EndpointSummary("Brand list")]
    [ProducesResponseType(typeof(IReadOnlyList<BrandResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<BrandResponseDto>>> GetBrands()
    {
        var brands = await _brandService.GetAllAsync();

        return Ok(brands);
    }

    // GET: api/brands/{id}
    [HttpGet("{id:int}")]
    [EndpointSummary("Returns brand by id")]
    [ProducesResponseType(typeof(BrandResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BrandResponseDto>> GetBrandById(int id)
    {
        var brand = await _brandService.GetByIdAsync(id);

        if (brand is null)
            return NotFound();

        return Ok(brand);
    }

    // POST: api/brands
    [HttpPost]
    [EndpointSummary("Creates new brand")]
    [ProducesResponseType(typeof(BrandResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<BrandResponseDto>> AddBrand(BrandDto dto)
    {
        _logger.LogDebug(
            "Adding brand: {Name}",
            dto.Name);

        var result = await _brandService.AddAsync(dto);

        if (!result.Success)
        {
            return Problem(
                title: result.Error,
                detail: result.Error,
                statusCode: result.StatusCode);
        }

        var brand = result.Brand!;

        return CreatedAtAction(
            nameof(GetBrandById),
            new { id = brand.Id },
            brand);
    }

    // PUT: api/brands/{id}
    [HttpPut("{id:int}")]
    [EndpointSummary("Updates existing brand")]
    [ProducesResponseType(typeof(BrandResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<BrandResponseDto>> UpdateBrand(
        int id,
        BrandDto dto)
    {
        var result = await _brandService.UpdateAsync(id, dto);

        if (!result.Success)
        {
            return Problem(
                title: result.Error,
                detail: result.Error,
                statusCode: result.StatusCode);
        }

        return Ok(result.Brand);
    }

    // DELETE: api/brands/{id}
    [HttpDelete("{id:int}")]
    [EndpointSummary("Deletes existing brand")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteBrand(int id)
    {
        var deleted = await _brandService.DeleteAsync(id);

        if (!deleted)
            return NotFound();

        return NoContent();
    }
}