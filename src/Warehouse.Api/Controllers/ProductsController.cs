using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Warehouse.Application.DTO;
using Warehouse.Application.Interfaces;
using Warehouse.Domain.Entities;
using Warehouse.Application.DTO;
using Warehouse.Infrastructure.Data;

namespace Warehouse.Api.Controllers;

[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly WarehouseDbContext _context;
    public ProductsController(
        WarehouseDbContext context)
    {
        _context = context;
    }
    // GET: api/products/
    [HttpGet]
    [EndpointSummary("Product list")]

    public async Task<IActionResult> GetProducts()
    {
        var products = await _context.Products.ToListAsync();

        return Ok(products);
    }

    [HttpGet("{id}")]
    [EndpointSummary("Returns product by id")]
    [ProducesResponseType(typeof(Product), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Product>> GetProductById(Guid id)
    {
        var product = await _context.Products.FindAsync(id);

        if (product == null)
            return NotFound();

        return product;
    }
    // POST: api/products/add
    [HttpPost("add")]
    [EndpointSummary("Inserts product into database")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<Product>> AddProduct(AddProductDto dto)
    {
        var existingProduct = await _context.Products.FirstOrDefaultAsync(x => x.Name == dto.Name);
        var existingEan = await _context.Products.FirstOrDefaultAsync(x => x.Ean == dto.Ean);

        if (existingEan != null)
            return Problem(
                title: "Product with this ean already exists",
                detail: "Product with this ean already exists.",
                statusCode: StatusCodes.Status409Conflict);

        if (existingProduct != null)
            return Problem(
                title: "Product with this name already exists",
                detail: "Product with this name already exists.",
                statusCode: StatusCodes.Status409Conflict);

        var categoryExists = await _context.Categories
            .AnyAsync(x => x.Id == dto.CategoryId);

        if (!categoryExists)
        {
            return Problem(
                title: "Category does not exist",
                detail: $"Category with id {dto.CategoryId} does not exist.",
                statusCode: StatusCodes.Status404NotFound);
        }

        var brandExists = await _context.Brands
            .AnyAsync(x => x.Id == dto.BrandId);

        if (!brandExists)
        {
            return Problem(
                title: "Brand does not exist",
                detail: $"Brand with id {dto.BrandId} does not exist.",
                statusCode: StatusCodes.Status404NotFound);
        }

        var product = new Product(dto.Name, dto.Ean)
        {
            CategoryId = dto.CategoryId,
            BrandId = dto.BrandId
        };


        _context.Products.Add(product);
        await _context.SaveChangesAsync();

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
    // DELETE: api/products/delete/id
    [HttpDelete("delete/{id}")]
    [EndpointSummary("Deletes product from database")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteProduct(Guid? id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null)
            return NotFound();

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();

        return NoContent();
    }
    // Put: api/products/update
    [HttpPut("update/{id}")]
    [EndpointSummary("Updates product")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<Product>> UpdateProduct(
        Guid id,
        UpdateProductDto dto)
    {
        var product = await _context.Products.FindAsync(id);

        if (product == null)
            return NotFound();

        var categoryExists = await _context.Categories
            .AnyAsync(x => x.Id == dto.CategoryId);

        if (!categoryExists)
        {
            return Problem(
                title: "Category does not exist",
                detail: $"Category with id {dto.CategoryId} does not exist.",
                statusCode: StatusCodes.Status404NotFound);
        }

        var brandExists = await _context.Brands
            .AnyAsync(x => x.Id == dto.BrandId);

        if (!brandExists)
        {
            return Problem(
                title: "Brand does not exist",
                detail: $"Brand with id {dto.BrandId} does not exist.",
                statusCode: StatusCodes.Status404NotFound);
        }

        var existingProduct = await _context.Products
            .FirstOrDefaultAsync(x => x.Name == dto.Name && x.Id != id);

        if (existingProduct != null)
        {
            return Problem(
                title: "Product with this name already exists",
                detail: "Product with this name already exists.",
                statusCode: StatusCodes.Status409Conflict);
        }

        var existingEan = await _context.Products
            .FirstOrDefaultAsync(x => x.Ean == dto.Ean && x.Id != id);

        if (existingEan != null)
        {
            return Problem(
                title: "Product with this ean already exists",
                detail: "Product with this ean already exists.",
                statusCode: StatusCodes.Status409Conflict);
        }

        product.Update(
            dto.Name,
            dto.Ean,
            dto.CategoryId,
            dto.BrandId);

        await _context.SaveChangesAsync();

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