using Warehouse.Application.DTO.Product;

namespace Warehouse.Application.Interfaces;

public interface IProductService
{
    Task<IReadOnlyList<ProductResponseDto>> GetAllAsync();
    Task<ProductResponseDto?> GetByIdAsync(int id);
    Task<ProductOperationResult> AddAsync(ProductDto dto);
    Task<ProductOperationResult> UpdateAsync(int id, ProductDto dto);
    Task<bool> DeleteAsync(int id);
}

public record ProductOperationResult(ProductResponseDto? Product, string? Error, int? StatusCode)
{
    public bool Success => Product != null;
    public static ProductOperationResult Ok(ProductResponseDto product) =>
        new(product, null, null);
    public static ProductOperationResult Fail(string error, int statusCode) =>
        new(null, error, statusCode);
}