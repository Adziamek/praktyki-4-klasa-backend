using Warehouse.Domain.Entities;

namespace Warehouse.Application.Interfaces;

public interface IUserService
{
    Task<IReadOnlyList<User>> GetAllAsync();
}