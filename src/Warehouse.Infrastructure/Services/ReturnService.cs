using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Warehouse.Application.Common;
using Warehouse.Application.DTO.ReturnRequest;
using Warehouse.Application.Interfaces;
using Warehouse.Domain.Entities;
using Warehouse.Infrastructure.Data;

namespace Warehouse.Infrastructure.Services;

public class ReturnRequestService : IReturnRequestService
{
    private readonly WarehouseDbContext _context;

    public ReturnRequestService(WarehouseDbContext context)
    {
        _context = context;
    }

    private static ReturnRequestResponseDto MapToDto(
        ReturnRequest returnRequest)
    {
        return new ReturnRequestResponseDto
        {
            Id = returnRequest.Id,
            OrderId = returnRequest.OrderId,
            Status = returnRequest.Status,
            Reason = returnRequest.Reason,
            CreatedAt = returnRequest.CreatedAt,

            Items = returnRequest.Items
                .Select(x => new ReturnItemResponseDto
                {
                    Id = x.Id,
                    OrderItemId = x.OrderItemId,
                    ProductId = x.OrderItem.ProductId,
                    Quantity = x.Quantity
                })
                .ToList()
        };
    }

    public async Task<IReadOnlyList<ReturnRequestResponseDto>> GetAllAsync()
    {
        var returnRequests = await _context.ReturnRequests
            .AsNoTracking()
            .Include(x => x.Items)
                .ThenInclude(x => x.OrderItem)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();

        return returnRequests
            .Select(MapToDto)
            .ToList();
    }

    public async Task<ReturnRequestResponseDto?> GetByIdAsync(int id)
    {
        var returnRequest = await _context.ReturnRequests
            .AsNoTracking()
            .Include(x => x.Items)
                .ThenInclude(x => x.OrderItem)
            .FirstOrDefaultAsync(x => x.Id == id);

        return returnRequest is null
            ? null
            : MapToDto(returnRequest);
    }

    public async Task<OperationResult<ReturnRequestResponseDto>> AddAsync(
        ReturnRequestDto dto)
    {
        if (dto.OrderId <= 0)
        {
            return OperationResult<ReturnRequestResponseDto>.Fail(
                "OrderId must be greater than 0.",
                StatusCodes.Status400BadRequest);
        }

        if (string.IsNullOrWhiteSpace(dto.Reason))
        {
            return OperationResult<ReturnRequestResponseDto>.Fail(
                "Return reason is required.",
                StatusCodes.Status400BadRequest);
        }

        if (dto.Items.Count == 0)
        {
            return OperationResult<ReturnRequestResponseDto>.Fail(
                "Return must contain at least one item.",
                StatusCodes.Status400BadRequest);
        }

        var order = await _context.CustomerOrders
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == dto.OrderId);

        if (order is null)
        {
            return OperationResult<ReturnRequestResponseDto>.Fail(
                $"Order with id {dto.OrderId} does not exist.",
                StatusCodes.Status404NotFound);
        }

        var duplicateOrderItemIds = dto.Items
            .GroupBy(x => x.OrderItemId)
            .Where(x => x.Count() > 1)
            .Select(x => x.Key)
            .ToList();

        if (duplicateOrderItemIds.Count > 0)
        {
            return OperationResult<ReturnRequestResponseDto>.Fail(
                "The same order item cannot appear more than once in a return.",
                StatusCodes.Status400BadRequest);
        }

        foreach (var item in dto.Items)
        {
            if (item.Quantity <= 0)
            {
                return OperationResult<ReturnRequestResponseDto>.Fail(
                    "Return quantity must be greater than 0.",
                    StatusCodes.Status400BadRequest);
            }
        }

        var orderItemIds = dto.Items
            .Select(x => x.OrderItemId)
            .Distinct()
            .ToList();

        var orderItems = await _context.OrderItems
            .Where(x =>
                x.OrderId == dto.OrderId &&
                orderItemIds.Contains(x.Id))
            .ToDictionaryAsync(x => x.Id);

        foreach (var orderItemId in orderItemIds)
        {
            if (!orderItems.ContainsKey(orderItemId))
            {
                return OperationResult<ReturnRequestResponseDto>.Fail(
                    $"Order item with id {orderItemId} does not belong to order {dto.OrderId}.",
                    StatusCodes.Status400BadRequest);
            }
        }

        var alreadyReturnedQuantities = await _context.ReturnItems
            .Where(x => orderItemIds.Contains(x.OrderItemId))
            .GroupBy(x => x.OrderItemId)
            .Select(g => new
            {
                OrderItemId = g.Key,
                Quantity = g.Sum(x => x.Quantity)
            })
            .ToDictionaryAsync(
                x => x.OrderItemId,
                x => x.Quantity);

        foreach (var itemDto in dto.Items)
        {
            var orderItem = orderItems[itemDto.OrderItemId];

            var alreadyReturned =
                alreadyReturnedQuantities.GetValueOrDefault(
                    itemDto.OrderItemId,
                    0);

            var remainingQuantity =
                orderItem.Quantity - alreadyReturned;

            if (itemDto.Quantity > remainingQuantity)
            {
                return OperationResult<ReturnRequestResponseDto>.Fail(
                    $"Cannot return {itemDto.Quantity} units of order item " +
                    $"{itemDto.OrderItemId}. " +
                    $"Available for return: {remainingQuantity}.",
                    StatusCodes.Status409Conflict);
            }
        }

        var returnRequest = new ReturnRequest
        {
            OrderId = dto.OrderId,
            Status = "Pending",
            Reason = dto.Reason.Trim(),
            CreatedAt = DateTime.UtcNow
        };

        foreach (var itemDto in dto.Items)
        {
            returnRequest.Items.Add(new ReturnItem
            {
                OrderItemId = itemDto.OrderItemId,
                Quantity = itemDto.Quantity
            });
        }

        _context.ReturnRequests.Add(returnRequest);

        await _context.SaveChangesAsync();

        await _context.Entry(returnRequest)
            .Collection(x => x.Items)
            .Query()
            .Include(x => x.OrderItem)
            .LoadAsync();

        return OperationResult<ReturnRequestResponseDto>.Ok(
            MapToDto(returnRequest));
    }
}