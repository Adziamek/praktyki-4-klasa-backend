using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Warehouse.Application.DTO.Location;
using Warehouse.Application.Interfaces;
using Warehouse.Domain.Entities;
using Warehouse.Infrastructure.Data;

namespace Warehouse.Infrastructure.Services;

public class LocationService : ILocationService
{
    private readonly WarehouseDbContext _context;

    public LocationService(WarehouseDbContext context)
    {
        _context = context;
    }

    private static LocationResponseDto MapToDto(Location location)
    {
        return new LocationResponseDto
        {
            Id = location.Id,
            Code = location.Code,
            WarehouseCode = location.CWarehouse.Code,
            Name = location.Name,
            IsActive = location.Active
        };
    }

    public async Task<IReadOnlyList<LocationResponseDto>> GetAllAsync()
    {
        var locations = await _context.Locations
            .AsNoTracking()
            .Include(x => x.CWarehouse)
            .ToListAsync();

        return locations.Select(MapToDto).ToList();
    }

    public async Task<LocationResponseDto?> GetByIdAsync(int id)
    {
        var location = await _context.Locations
            .AsNoTracking()
            .Include(x => x.CWarehouse)
            .FirstOrDefaultAsync(x => x.Id == id);

        return location is null ? null : MapToDto(location);
    }

    public async Task<LocationOperationResult> AddAsync(LocationDto dto)
    {
        var warehouse = await _context.Warehouses
            .FirstOrDefaultAsync(x => x.Code == dto.WarehouseCode);

        if (warehouse is null)
        {
            return LocationOperationResult.Fail(
                $"Warehouse with code {dto.WarehouseCode} does not exist.",
                StatusCodes.Status404NotFound);
        }

        var existingName = await _context.Locations
            .AnyAsync(x => x.Name == dto.Name);

        if (existingName)
        {
            return LocationOperationResult.Fail(
                "Location with this name already exists.",
                StatusCodes.Status409Conflict);
        }

        var existingCode = await _context.Locations
            .AnyAsync(x => x.Code == dto.Code);

        if (existingCode)
        {
            return LocationOperationResult.Fail(
                "Location with this code already exists.",
                StatusCodes.Status409Conflict);
        }

        var location = new Location
        {
            WarehouseId = warehouse.Id,
            CWarehouse = warehouse,
            Code = dto.Code,
            Name = dto.Name,
            Active = dto.IsActive
        };

        _context.Locations.Add(location);
        await _context.SaveChangesAsync();

        return LocationOperationResult.Ok(MapToDto(location));
    }

    public async Task<LocationOperationResult> UpdateAsync(
        int id,
        LocationDto dto)
    {
        var location = await _context.Locations
            .Include(x => x.CWarehouse)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (location is null)
        {
            return LocationOperationResult.Fail(
                "Location does not exist.",
                StatusCodes.Status404NotFound);
        }

        var warehouse = await _context.Warehouses
            .FirstOrDefaultAsync(x => x.Code == dto.WarehouseCode);

        if (warehouse is null)
        {
            return LocationOperationResult.Fail(
                $"Warehouse with code {dto.WarehouseCode} does not exist.",
                StatusCodes.Status404NotFound);
        }

        var existingName = await _context.Locations
            .AnyAsync(x => x.Name == dto.Name && x.Id != id);

        if (existingName)
        {
            return LocationOperationResult.Fail(
                "Location with this name already exists.",
                StatusCodes.Status409Conflict);
        }

        var existingCode = await _context.Locations
            .AnyAsync(x => x.Code == dto.Code && x.Id != id);

        if (existingCode)
        {
            return LocationOperationResult.Fail(
                "Location with this code already exists.",
                StatusCodes.Status409Conflict);
        }

        location.Code = dto.Code;
        location.Name = dto.Name;
        location.Active = dto.IsActive;
        location.WarehouseId = warehouse.Id;
        location.CWarehouse = warehouse;

        await _context.SaveChangesAsync();

        return LocationOperationResult.Ok(MapToDto(location));
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var location = await _context.Locations
            .FirstOrDefaultAsync(x => x.Id == id);

        if (location is null)
            return false;

        _context.Locations.Remove(location);
        await _context.SaveChangesAsync();

        return true;
    }
}