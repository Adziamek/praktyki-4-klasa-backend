using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
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

    public async Task<IReadOnlyList<Location>> GetAllAsync()
    {
        return await _context.Locations
            .AsNoTracking()
            .ToListAsync();
    }

}