using Warehouse.Application.DTO.Location;

namespace Warehouse.Application.Interfaces;

public interface ILocationService
{
    Task<IReadOnlyList<LocationResponseDto>> GetAllAsync();
    Task<LocationResponseDto?> GetByIdAsync(int id);
    Task<LocationOperationResult> AddAsync(LocationDto dto);
    Task<LocationOperationResult> UpdateAsync(int id, LocationDto dto);
    Task<LocationOperationResult> DeleteAsync(int id);
}

public record LocationOperationResult(
    LocationResponseDto? Location,
    string? Error,
    int? StatusCode)
{
    public bool Success => Error == null;

    public static LocationOperationResult Ok() =>
        new(null, null, null);

    public static LocationOperationResult Ok(LocationResponseDto location) =>
        new(location, null, null);

    public static LocationOperationResult Fail(string error, int statusCode) =>
        new(null, error, statusCode);
}