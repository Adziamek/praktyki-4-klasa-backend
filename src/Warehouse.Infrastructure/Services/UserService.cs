using Microsoft.EntityFrameworkCore;
using Warehouse.Application.Interfaces;
using Warehouse.Domain.Entities;
using Warehouse.Infrastructure.Data;

namespace Warehouse.Infrastructure.Services;

public class UserService : IUserService
{
    private readonly WarehouseDbContext _context;

    public UserService(WarehouseDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<User>> GetAllAsync()
    {
        return await _context.Users
            .AsNoTracking()
            .ToListAsync();
    }
}