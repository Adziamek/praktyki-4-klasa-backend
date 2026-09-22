using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Warehouse.Application.DTO.Brand;
using Warehouse.Application.Interfaces;
using Warehouse.Domain.Entities;
using Warehouse.Infrastructure.Data;

namespace Warehouse.Infrastructure.Services;

public class BrandService : IBrandService
{
    private readonly WarehouseDbContext _context;

    public BrandService(WarehouseDbContext context)
    {
        _context = context;
    }
    
    private static BrandResponseDto MapToDto(Brand brand)
    {
        return new BrandResponseDto
        {
            Id = brand.Id,
            Name = brand.Name,
            Description = brand.Description
        };
    }

    public async Task<IReadOnlyList<BrandResponseDto>> GetAllAsync()
    {
        var brands = await _context.Brands
            .AsNoTracking()
            .ToListAsync();

        return brands.Select(MapToDto).ToList();    
    }

    public async Task<BrandResponseDto?> GetByIdAsync(int id)
    {
        var brand = await _context.Brands
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);

        return brand is null ? null : MapToDto(brand);
    }

    public async Task<BrandOperationResult> AddAsync(BrandDto dto)
    {
        var existingBrand = await _context.Brands
            .AnyAsync(x => x.Name == dto.Name);

        if (existingBrand)
        {
            return BrandOperationResult.Fail(
                "Brand with this name already exists.",
                StatusCodes.Status409Conflict);
        }

        var brand = new Brand
        {
            Name = dto.Name,
            Description = dto.Description
        };
        
        _context.Brands.Add(brand);
        await _context.SaveChangesAsync();

        return BrandOperationResult.Ok(MapToDto(brand));
    }

    public async Task<BrandOperationResult?> UpdateAsync(int id, BrandDto dto)
    {
        var brand = await _context.Brands.FindAsync(id);
        if (brand is null) return null;
        
        var existingBrand = await _context.Brands
            .AnyAsync(x => x.Name == dto.Name && x.Id != id);

        if (existingBrand)
        {
            return BrandOperationResult.Fail(
                "Brand with this name already exists.",
                StatusCodes.Status409Conflict);
        }
        
        brand.Name = dto.Name;
        brand.Description = dto.Description;

        await _context.SaveChangesAsync();
        
        return BrandOperationResult.Ok(MapToDto(brand));
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var brand = await _context.Brands
            .FirstOrDefaultAsync(x => x.Id == id);

        if (brand == null)
            return false;

        _context.Brands.Remove(brand);
        await _context.SaveChangesAsync();

        return true;
    }
}