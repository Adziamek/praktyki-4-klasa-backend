using Warehouse.Application.DTO.Category;

namespace Warehouse.Application.Interfaces;

public interface ICategoryService
{
    Task<IReadOnlyList<CategoryResponseDto>> GetAllAsync();
    Task<CategoryResponseDto?> GetByIdAsync(int id);
    Task<CategoryOperationResult> AddAsync(CategoryDto brand);
    Task<CategoryOperationResult?> UpdateAsync(int id, CategoryDto brand);
    Task<bool> DeleteAsync(int id);
}
public record CategoryOperationResult(CategoryResponseDto? Category, string? Error, int? StatusCode)
{
    public bool Success => Category != null;
    public static CategoryOperationResult Ok(CategoryResponseDto category) =>
        new(category, null, null);
    public static CategoryOperationResult Fail(string error, int statusCode) =>
        new(null, error, statusCode);
}