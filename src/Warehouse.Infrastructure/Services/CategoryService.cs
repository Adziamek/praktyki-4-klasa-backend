using Microsoft.EntityFrameworkCore;
using Warehouse.Application.Interfaces;
using Warehouse.Domain.Entities;
using Warehouse.Infrastructure.Data;

namespace Warehouse.Infrastructure.Services;

public class CategoryService : ICategoryService
{
    private readonly WarehouseDbContext _context;

    public CategoryService(WarehouseDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Category>> GetAllAsync()
    {
        return await _context.Categories.ToListAsync();
    }

    public async Task<Category?> GetByIdAsync(int id)
    {
        return await _context.Categories.FindAsync(id);
    }

    public async Task<Category> CreateAsync(Category category)
    {
        _context.Categories.Add(category);
        await _context.SaveChangesAsync();
        return category;
    }

    public async Task<Category?> UpdateAsync(int id, Category category)
    {
        var existing = await _context.Categories.FindAsync(id);
        if (existing is null) return null;

        existing.Name = category.Name;
        existing.Description = category.Description;

        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var existing = await _context.Categories.FindAsync(id);
        if (existing is null) return false;

        _context.Categories.Remove(existing);
        await _context.SaveChangesAsync();
        return true;
    }
}