using Microsoft.AspNetCore.Mvc;
using Warehouse.Application.DTO.Product;
using Warehouse.Application.Interfaces;
using Warehouse.Domain.Entities;

namespace Warehouse.Api.Controllers;

[ApiController]
[Route("api/products")]
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
    public async Task<ActionResult<IReadOnlyList<Product>>> GetProducts()
    {
        var products = await _productService.GetAllAsync();
        return Ok(products);
    }

    // GET: api/products/{id}
    [HttpGet("{id:guid}")]
    [EndpointSummary("Returns product by id")]
    [ProducesResponseType(typeof(Product), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Product>> GetProductById(Guid id)
    {
        var product = await _productService.GetByIdAsync(id);

        if (product == null)
            return NotFound();

        return Ok(product);
    }

    // POST: api/products/add
    [HttpPost("add")]
    [EndpointSummary("Inserts product into database")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<Product>> AddProduct(ProductDto dto)
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
            new
            {
                product.Id,
                product.Name,
                product.Ean,
                product.CategoryId,
                product.BrandId
            });
    }

    // DELETE: api/products/delete/{id}
    [HttpDelete("delete/{id:guid}")]
    [EndpointSummary("Deletes product from database")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteProduct(Guid id)
    {
        var deleted = await _productService.DeleteAsync(id);

        if (!deleted)
            return NotFound();

        return NoContent();
    }

    // PUT: api/products/update/{id}
    [HttpPut("update/{id:guid}")]
    [EndpointSummary("Updates product")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<Product>> UpdateProduct(
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

        var product = result.Product!;

        return Ok(new
        {
            product.Id,
            product.Name,
            product.Ean,
            product.CategoryId,
            product.BrandId
        });
    }
}