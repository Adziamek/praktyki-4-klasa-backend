using Warehouse.Application.DTO.CustomerOrder;
using Warehouse.Application.Interfaces;
using Warehouse.Domain.Entities;
using Warehouse.Infrastructure.Data;

namespace Warehouse.Infrastructure.Services;

public class CustomerOrderService : ICustomerOrderService
{
    private readonly WarehouseDbContext _context;

    public CustomerOrderService(WarehouseDbContext context)
    {
        _context = context;
    }
    private static CustomerOrderResponseDto MapToDto(CustomerOrder customerOrder)
    {
        return new CustomerOrderResponseDto
        {
            Id = customerOrder.Id,
            UserId = customerOrder.UserId,
            Status = customerOrder.Status,
            CreatedAt = customerOrder.CreatedAt
        };
    }
}