using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Warehouse.Application.Common;
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

    private static ProductResponseDto MapToDto(
        Product product,
        int quantity = 0,
        List<ProductLocationDto>? locations = null)
    {
        return new ProductResponseDto
        {
            Id = product.Id,
            Name = product.Name,
            Ean = product.Ean,
            CategoryId = product.CategoryId,
            Category = product.Category.Name,
            BrandId = product.BrandId,
            Brand = product.Brand.Name,
            Price = product.Price,
            Quantity = quantity,
            Locations = locations ?? []
        };
    }
    public async Task<IReadOnlyList<ProductResponseDto>> GetAllAsync()
    {
        var products = await _context.Products
            .AsNoTracking()
            .Include(x => x.Category)
            .Include(x => x.Brand)
            .ToListAsync();

        var productIds = products
            .Select(x => x.Id)
            .ToList();

        var stockQuantities = await _context.Stocks
            .Where(x => productIds.Contains(x.ProductId))
            .GroupBy(x => x.ProductId)
            .Select(g => new
            {
                ProductId = g.Key,
                Quantity = g.Sum(x => x.Quantity)
            })
            .ToDictionaryAsync(
                x => x.ProductId,
                x => x.Quantity);

        return products
            .Select(product => MapToDto(
                product,
                stockQuantities.GetValueOrDefault(product.Id)))
            .ToList();
    }

    public async Task<ProductResponseDto?> GetByIdAsync(int id)
    {
        var product = await _context.Products
            .AsNoTracking()
            .Include(x => x.Category)
            .Include(x => x.Brand)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (product is null)
            return null;

        var locations = await _context.Stocks
            .AsNoTracking()
            .Where(x => x.ProductId == id)
            .Select(x => new ProductLocationDto
            {
                LocationId = x.LocationId,
                Quantity = x.Quantity
            })
            .ToListAsync();

        var quantity = locations.Sum(x => x.Quantity);

        return MapToDto(
            product,
            quantity,
            locations);
    }

        public async Task<OperationResult<ProductResponseDto>> AddAsync(ProductDto dto)
{
    var existingProduct = await _context.Products
        .AnyAsync(x => x.Name == dto.Name);

    if (existingProduct)
    {
        return OperationResult<ProductResponseDto>.Fail(
            "Product with this name already exists.",
            StatusCodes.Status409Conflict);
    }

    var existingEan = await _context.Products
        .AnyAsync(x => x.Ean == dto.Ean);

    if (existingEan)
    {
        return OperationResult<ProductResponseDto>.Fail(
            "Product with this EAN already exists.",
            StatusCodes.Status409Conflict);
    }

    var categoryExists = await _context.Categories
        .AnyAsync(x => x.Id == dto.CategoryId);

    if (!categoryExists)
    {
        return OperationResult<ProductResponseDto>.Fail(
            $"Category with id {dto.CategoryId} does not exist.",
            StatusCodes.Status404NotFound);
    }

    var brandExists = await _context.Brands
        .AnyAsync(x => x.Id == dto.BrandId);

    if (!brandExists)
    {
        return OperationResult<ProductResponseDto>.Fail(
            $"Brand with id {dto.BrandId} does not exist.",
            StatusCodes.Status404NotFound);
    }

    foreach (var location in dto.Locations)
    {
        var locationExists = await _context.Locations
            .AnyAsync(x => x.Id == location.LocationId && x.Active);

        if (!locationExists)
        {
            return OperationResult<ProductResponseDto>.Fail(
                $"Location with id {location.LocationId} does not exist.",
                StatusCodes.Status404NotFound);
        }
    }

    var product = new Product
    {
        Name = dto.Name,
        Ean = dto.Ean,
        CategoryId = dto.CategoryId,
        BrandId = dto.BrandId,
        Price = dto.Price
    };

    _context.Products.Add(product);

    await _context.SaveChangesAsync();

    foreach (var location in dto.Locations)
    {
        var stock = new Stock
        {
            ProductId = product.Id,
            LocationId = location.LocationId,
            Quantity = location.Quantity
        };

        _context.Stocks.Add(stock);
    }

    await _context.SaveChangesAsync();

    var createdProduct = await _context.Products
        .AsNoTracking()
        .Include(x => x.Category)
        .Include(x => x.Brand)
        .FirstAsync(x => x.Id == product.Id);

    var locations = await _context.Stocks
        .AsNoTracking()
        .Where(x => x.ProductId == product.Id)
        .Select(x => new ProductLocationDto
        {
            LocationId = x.LocationId,
            Quantity = x.Quantity
        })
        .ToListAsync();

    var quantity = locations.Sum(x => x.Quantity);

    return OperationResult<ProductResponseDto>.Ok(
        MapToDto(
            createdProduct,
            quantity,
            locations));
}

    public async Task<OperationResult<ProductResponseDto>> UpdateAsync(
    int id,
    ProductDto dto)
{
    var product = await _context.Products
        .FirstOrDefaultAsync(x => x.Id == id);

    if (product is null)
    {
        return OperationResult<ProductResponseDto>.Fail(
            "Product does not exist.",
            StatusCodes.Status404NotFound);
    }

    var categoryExists = await _context.Categories
        .AnyAsync(x => x.Id == dto.CategoryId);

    if (!categoryExists)
    {
        return OperationResult<ProductResponseDto>.Fail(
            $"Category with id {dto.CategoryId} does not exist.",
            StatusCodes.Status404NotFound);
    }

    var brandExists = await _context.Brands
        .AnyAsync(x => x.Id == dto.BrandId);

    if (!brandExists)
    {
        return OperationResult<ProductResponseDto>.Fail(
            $"Brand with id {dto.BrandId} does not exist.",
            StatusCodes.Status404NotFound);
    }

    var existingProduct = await _context.Products
        .AnyAsync(x => x.Name == dto.Name && x.Id != id);

    if (existingProduct)
    {
        return OperationResult<ProductResponseDto>.Fail(
            "Product with this name already exists.",
            StatusCodes.Status409Conflict);
    }

    var existingEan = await _context.Products
        .AnyAsync(x => x.Ean == dto.Ean && x.Id != id);

    if (existingEan)
    {
        return OperationResult<ProductResponseDto>.Fail(
            "Product with this EAN already exists.",
            StatusCodes.Status409Conflict);
    }

    // Sprawdzamy wszystkie lokalizacje
    foreach (var location in dto.Locations)
    {
        var locationExists = await _context.Locations
            .AnyAsync(x => x.Id == location.LocationId && x.Active);

        if (!locationExists)
        {
            return OperationResult<ProductResponseDto>.Fail(
                $"Location with id {location.LocationId} does not exist.",
                StatusCodes.Status404NotFound);
        }
    }

    // Aktualizacja produktu
    product.Name = dto.Name;
    product.Ean = dto.Ean;
    product.CategoryId = dto.CategoryId;
    product.BrandId = dto.BrandId;
    product.Price = dto.Price;

    // Usuwamy stare Stocki
    var existingStocks = await _context.Stocks
        .Where(x => x.ProductId == id)
        .ToListAsync();

    _context.Stocks.RemoveRange(existingStocks);

    // Dodajemy nowe Stocki
    foreach (var location in dto.Locations)
    {
        var stock = new Stock
        {
            ProductId = id,
            LocationId = location.LocationId,
            Quantity = location.Quantity
        };

        _context.Stocks.Add(stock);
    }

    await _context.SaveChangesAsync();

    var updatedProduct = await _context.Products
        .AsNoTracking()
        .Include(x => x.Category)
        .Include(x => x.Brand)
        .FirstAsync(x => x.Id == id);

    var locations = await _context.Stocks
        .AsNoTracking()
        .Where(x => x.ProductId == id)
        .Select(x => new ProductLocationDto
        {
            LocationId = x.LocationId,
            Quantity = x.Quantity
        })
        .ToListAsync();

    var quantity = locations.Sum(x => x.Quantity);

    return OperationResult<ProductResponseDto>.Ok(
        MapToDto(
            updatedProduct,
            quantity,
            locations));
}

    public async Task<OperationResult> DeleteAsync(int id)
    {
        var product = await _context.Products
            .FirstOrDefaultAsync(x => x.Id == id);

        if (product is null)
            return OperationResult.Fail("Product not found.", StatusCodes.Status404NotFound);

        var ordered = await _context.OrderItems.AnyAsync(x => x.ProductId == id);

        if (ordered)
            return OperationResult.Fail("Cannot delete product because it is ordered.", StatusCodes.Status409Conflict);

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();

        return OperationResult.Ok();
    }
}