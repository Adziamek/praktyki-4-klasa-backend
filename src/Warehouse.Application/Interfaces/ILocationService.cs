using Warehouse.Application.DTO.Location;
using Warehouse.Domain.Entities;

namespace Warehouse.Application.Interfaces;

public interface ILocationService
{
    Task<IReadOnlyList<Location>> GetAllAsync();

}