using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
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

    private static CustomerOrderResponseDto MapToDto(
        CustomerOrder customerOrder)
    {
        return new CustomerOrderResponseDto
        {
            Id = customerOrder.Id,
            UserId = customerOrder.UserId,
            Status = customerOrder.Status,
            CreatedAt = customerOrder.CreatedAt,

            Items = customerOrder.Items
                .Select(x => new OrderItemResponseDto
                {
                    Id = x.Id,
                    ProductId = x.ProductId,
                    Quantity = x.Quantity,
                    UnitPrice = x.UnitPrice
                })
                .ToList()
        };
    }

    public async Task<IReadOnlyList<CustomerOrderResponseDto>> GetAllAsync()
    {
        var customerOrders = await _context.CustomerOrders
            .AsNoTracking()
            .Include(x => x.Items)
            .ToListAsync();

        return customerOrders
            .Select(MapToDto)
            .ToList();
    }

    public async Task<CustomerOrderResponseDto?> GetByIdAsync(int id)
    {
        var customerOrder = await _context.CustomerOrders
            .AsNoTracking()
            .Include(x => x.Items)
            .FirstOrDefaultAsync(x => x.Id == id);

        return customerOrder is null
            ? null
            : MapToDto(customerOrder);
    }

    public async Task<CustomerOrderOperationResult> AddAsync(CustomerOrderDto dto)
    {
        if (dto.Items.Count == 0)
        {
            return CustomerOrderOperationResult.Fail(
                "Order must contain at least one item.",StatusCodes.Status400BadRequest);
        }
        foreach (var item in dto.Items)
        {
            if (item.Quantity <= 0)
            {
                return CustomerOrderOperationResult.Fail(
                    "Quantity must be greater than 0.",StatusCodes.Status400BadRequest);
            }
        }
        var productIds = dto.Items
            .Select(x => x.ProductId)
            .Distinct()
            .ToList();
        var products = await _context.Products
            .Where(x => productIds.Contains(x.Id))
            .ToDictionaryAsync(x => x.Id);
        foreach (var productId in productIds)
        {
            if (!products.ContainsKey(productId))
            {
                return CustomerOrderOperationResult.Fail(
                    $"Product with id {productId} does not exist.",StatusCodes.Status404NotFound);
            }
        }
        var stockQuantities = await _context.Stocks
            .Where(x => productIds.Contains(x.ProductId))
            .GroupBy(x => x.ProductId)
            .Select(g => new
            {
                ProductId = g.Key,
                Quantity = g.Sum(x => x.Quantity)
            })
            .ToDictionaryAsync(
                x => x.ProductId,
                x => x.Quantity);
        foreach (var item in dto.Items)
        {
            var availableQuantity =
                stockQuantities.GetValueOrDefault(
                    item.ProductId,
                    0);

            if (availableQuantity < item.Quantity)
            {
                return CustomerOrderOperationResult.Fail(
                    $"Not enough stock for product " +
                    $"{item.ProductId}. " +
                    $"Available: {availableQuantity}, " +
                    $"requested: {item.Quantity}.",StatusCodes.Status409Conflict);
            }
        }
        var customerOrder = new CustomerOrder
        {
            UserId = dto.UserId
        };
        foreach (var itemDto in dto.Items)
        {
            var product = products[itemDto.ProductId];

            customerOrder.Items.Add(new OrderItem
            {
                ProductId = product.Id,
                Quantity = itemDto.Quantity,
                UnitPrice = product.Price
            });
        }
        _context.CustomerOrders.Add(customerOrder);
        foreach (var itemDto in dto.Items)
        {
            var remainingQuantity = itemDto.Quantity;

            var stocks = await _context.Stocks
                .Where(x => x.ProductId == itemDto.ProductId)
                .OrderBy(x => x.Id)
                .ToListAsync();

            foreach (var stock in stocks)
            {
                if (remainingQuantity <= 0)
                    break;

                var quantityToTake = Math.Min(
                    stock.Quantity,
                    remainingQuantity);

                stock.Quantity -= quantityToTake;

                remainingQuantity -= quantityToTake;
            }
        }
        await _context.SaveChangesAsync();
        
        return CustomerOrderOperationResult.Ok(
            MapToDto(customerOrder));
    }
}