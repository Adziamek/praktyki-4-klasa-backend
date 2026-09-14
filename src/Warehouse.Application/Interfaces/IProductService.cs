using Warehouse.Domain.Entities;

namespace Warehouse.Application.Interfaces;

public interface IProductService
{
    Task<IReadOnlyList<Product>> GetAllAsync();
}