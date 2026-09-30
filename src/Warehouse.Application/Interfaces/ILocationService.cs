using Warehouse.Application.Common;
using Warehouse.Application.DTO.Location;

namespace Warehouse.Application.Interfaces;

public interface ILocationService
{
    Task<IReadOnlyList<LocationResponseDto>> GetAllAsync();
    Task<LocationResponseDto?> GetByIdAsync(int id);

    Task<OperationResult<LocationResponseDto>> AddAsync(LocationDto dto);
    Task<OperationResult<LocationResponseDto>> UpdateAsync(int id, LocationDto dto);

    Task<OperationResult> DeleteAsync(int id);
}