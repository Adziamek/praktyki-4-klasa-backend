using Microsoft.EntityFrameworkCore;
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

    public async Task<IEnumerable<Brand>> GetAllAsync()
    {
        return await _context.Brands.ToListAsync();
    }

    public async Task<Brand?> GetByIdAsync(int id)
    {
        return await _context.Brands.FindAsync(id);
    }

    public async Task<Brand> CreateAsync(Brand brand)
    {
        _context.Brands.Add(brand);
        await _context.SaveChangesAsync();
        return brand;
    }

    public async Task<Brand?> UpdateAsync(int id, Brand brand)
    {
        var existing = await _context.Brands.FindAsync(id);
        if (existing is null) return null;

        existing.Name = brand.Name;
        existing.Description = brand.Description;

        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var existing = await _context.Brands.FindAsync(id);
        if (existing is null) return false;

        _context.Brands.Remove(existing);
        await _context.SaveChangesAsync();
        return true;
    }
}