using Warehouse.Application.DTO.Brand;

namespace Warehouse.Application.Interfaces;

public interface IBrandService
{
    Task<IReadOnlyList<BrandResponseDto>> GetAllAsync();
    Task<BrandResponseDto?> GetByIdAsync(int id);
    Task<BrandOperationResult> AddAsync(BrandDto brand);
    Task<BrandOperationResult?> UpdateAsync(int id, BrandDto brand);
    Task<bool> DeleteAsync(int id);
}
public record BrandOperationResult(BrandResponseDto? Brand, string? Error, int? StatusCode)
{
    public bool Success => Brand != null;
    public static BrandOperationResult Ok(BrandResponseDto brand) =>
        new(brand, null, null);
    public static BrandOperationResult Fail(string error, int statusCode) =>
        new(null, error, statusCode);
}