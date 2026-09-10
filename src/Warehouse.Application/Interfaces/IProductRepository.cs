using Warehouse.Domain.Entities;

namespace Warehouse.Application.Interfaces;

public interface IProductRepository
{
    Task<IReadOnlyList<Product>> GetAllAsync();
}