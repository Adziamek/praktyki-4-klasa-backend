using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Warehouse.Application.DTO.Category;
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
    
    private static CategoryResponseDto MapToDto(Category category)
    {
        return new CategoryResponseDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description
        };
    }

    public async Task<IReadOnlyList<CategoryResponseDto>> GetAllAsync()
    {
        var categorys = await _context.Categories
            .AsNoTracking()
            .ToListAsync();

        return categorys.Select(MapToDto).ToList();    
    }

    public async Task<CategoryResponseDto?> GetByIdAsync(int id)
    {
        var category = await _context.Categories
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);

        return category is null ? null : MapToDto(category);
    }

    public async Task<CategoryOperationResult> AddAsync(CategoryDto dto)
    {
        var existingcategory = await _context.Categories
            .AnyAsync(x => x.Name == dto.Name);

        if (existingcategory)
        {
            return CategoryOperationResult.Fail(
                "Category with this name already exists.",
                StatusCodes.Status409Conflict);
        }

        var category = new Category
        {
            Name = dto.Name,
            Description = dto.Description
        };
        
        _context.Categories.Add(category);
        await _context.SaveChangesAsync();

        return CategoryOperationResult.Ok(MapToDto(category));
    }

    public async Task<CategoryOperationResult?> UpdateAsync(int id, CategoryDto dto)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category is null) return null;
        
        var existingCategory = await _context.Categories
            .AnyAsync(x => x.Name == dto.Name && x.Id != id);

        if (existingCategory)
        {
            return CategoryOperationResult.Fail(
                "Category with this name already exists.",
                StatusCodes.Status409Conflict);
        }
        
        category.Name = dto.Name;
        category.Description = dto.Description;

        await _context.SaveChangesAsync();
        
        return CategoryOperationResult.Ok(MapToDto(category));
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var category = await _context.Categories
            .FirstOrDefaultAsync(x => x.Id == id);

        if (category == null)
            return false;

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();

        return true;
    }
}