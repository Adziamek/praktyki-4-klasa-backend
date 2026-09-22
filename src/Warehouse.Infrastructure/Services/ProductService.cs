using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Warehouse.Application.DTO.Product;
using Warehouse.Application.Interfaces;
using Warehouse.Domain.Entities;
using Warehouse.Infrastructure.Data;

namespace Warehouse.Infrastructure.Services;

public class ProductService : IProductService
{
    private readonly WarehouseDbContext _context;

    public ProductService(WarehouseDbContext context)
    {
        _context = context;
    }
    
    private static ProductResponseDto MapToDto(Product product)
    {
        return new ProductResponseDto
        {
            Id = product.Id,
            Name = product.Name,
            Ean = product.Ean,
            CategoryId = product.CategoryId,
            BrandId = product.BrandId
        };
    }
    
    public async Task<IReadOnlyList<ProductResponseDto>> GetAllAsync()
    {
        var products = await _context.Products
            .AsNoTracking()
            .ToListAsync();

        return products.Select(MapToDto).ToList();
    }

   
    public async Task<ProductResponseDto?> GetByIdAsync(Guid id)
    {
        var product = await _context.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);

        return product is null ? null : MapToDto(product);
    }

    public async Task<ProductOperationResult> AddAsync(ProductDto dto)
    {
        var existingProduct = await _context.Products
            .AnyAsync(x => x.Name == dto.Name);

        if (existingProduct)
        {
            return ProductOperationResult.Fail(
                "Product with this name already exists.",
                StatusCodes.Status409Conflict);
        }

        var existingEan = await _context.Products
            .AnyAsync(x => x.Ean == dto.Ean);

        if (existingEan)
        {
            return ProductOperationResult.Fail(
                "Product with this EAN already exists.",
                StatusCodes.Status409Conflict);
        }

        var categoryExists = await _context.Categories
            .AnyAsync(x => x.Id == dto.CategoryId);

        if (!categoryExists)
        {
            return ProductOperationResult.Fail(
                $"Category with id {dto.CategoryId} does not exist.",
                StatusCodes.Status404NotFound);
        }

        var brandExists = await _context.Brands
            .AnyAsync(x => x.Id == dto.BrandId);

        if (!brandExists)
        {
            return ProductOperationResult.Fail(
                $"Brand with id {dto.BrandId} does not exist.",
                StatusCodes.Status404NotFound);
        }

        var product = new Product
        {
            Name = dto.Name,
            Ean = dto.Ean,
            CategoryId = dto.CategoryId,
            BrandId = dto.BrandId
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        return ProductOperationResult.Ok(MapToDto(product));
        
    }

    public async Task<ProductOperationResult> UpdateAsync(
        Guid id,
        ProductDto dto)
    {
        var product = await _context.Products
            .FirstOrDefaultAsync(x => x.Id == id);

        if (product == null)
        {
            return ProductOperationResult.Fail(
                "Product does not exist.",
                StatusCodes.Status404NotFound);
        }

        var categoryExists = await _context.Categories
            .AnyAsync(x => x.Id == dto.CategoryId);

        if (!categoryExists)
        {
            return ProductOperationResult.Fail(
                $"Category with id {dto.CategoryId} does not exist.",
                StatusCodes.Status404NotFound);
        }

        var brandExists = await _context.Brands
            .AnyAsync(x => x.Id == dto.BrandId);

        if (!brandExists)
        {
            return ProductOperationResult.Fail(
                $"Brand with id {dto.BrandId} does not exist.",
                StatusCodes.Status404NotFound);
        }

        var existingProduct = await _context.Products
            .AnyAsync(x => x.Name == dto.Name && x.Id != id);

        if (existingProduct)
        {
            return ProductOperationResult.Fail(
                "Product with this name already exists.",
                StatusCodes.Status409Conflict);
        }

        var existingEan = await _context.Products
            .AnyAsync(x => x.Ean == dto.Ean && x.Id != id);

        if (existingEan)
        {
            return ProductOperationResult.Fail(
                "Product with this EAN already exists.",
                StatusCodes.Status409Conflict);
        }

        product.Name = dto.Name;
        product.Ean = dto.Ean;
        product.CategoryId = dto.CategoryId;
        product.BrandId = dto.BrandId;

        await _context.SaveChangesAsync();

        return ProductOperationResult.Ok(MapToDto(product));
        
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var product = await _context.Products
            .FirstOrDefaultAsync(x => x.Id == id);

        if (product == null)
            return false;

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();

        return true;
    }
}