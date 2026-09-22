using Microsoft.AspNetCore.Mvc;
using Warehouse.Application.DTO.Product;
using Warehouse.Application.Interfaces;

namespace Warehouse.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(
        IProductService productService,
        ILogger<ProductsController> logger)
    {
        _productService = productService;
        _logger = logger;
    }

    // GET: api/products
    [HttpGet]
    [EndpointSummary("Product list")]
    [ProducesResponseType(typeof(IReadOnlyList<ProductResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ProductResponseDto>>> GetProducts()
    {
        var products = await _productService.GetAllAsync();

        return Ok(products);
    }

    // GET: api/products/{id}
    [HttpGet("{id:guid}")]
    [EndpointSummary("Returns product by id")]
    [ProducesResponseType(typeof(ProductResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductResponseDto>> GetProductById(Guid id)
    {
        var product = await _productService.GetByIdAsync(id);

        if (product is null)
            return NotFound();

        return Ok(product);
    }

    // POST: api/products
    [HttpPost]
    [EndpointSummary("Creates new product")]
    [ProducesResponseType(typeof(ProductResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ProductResponseDto>> AddProduct(ProductDto dto)
    {
        _logger.LogDebug(
            "Adding product: {Name}, EAN: {Ean}",
            dto.Name,
            dto.Ean);

        var result = await _productService.AddAsync(dto);

        if (!result.Success)
        {
            return Problem(
                title: result.Error,
                detail: result.Error,
                statusCode: result.StatusCode);
        }

        var product = result.Product!;

        return CreatedAtAction(
            nameof(GetProductById),
            new { id = product.Id },
            product);
    }

    // PUT: api/products/{id}
    [HttpPut("{id:guid}")]
    [EndpointSummary("Updates existing product")]
    [ProducesResponseType(typeof(ProductResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ProductResponseDto>> UpdateProduct(
        Guid id,
        ProductDto dto)
    {
        var result = await _productService.UpdateAsync(id, dto);

        if (!result.Success)
        {
            return Problem(
                title: result.Error,
                detail: result.Error,
                statusCode: result.StatusCode);
        }

        return Ok(result.Product);
    }

    // DELETE: api/products/{id}
    [HttpDelete("{id:guid}")]
    [EndpointSummary("Deletes existing product")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteProduct(Guid id)
    {
        var deleted = await _productService.DeleteAsync(id);

        if (!deleted)
            return NotFound();

        return NoContent();
    }
}