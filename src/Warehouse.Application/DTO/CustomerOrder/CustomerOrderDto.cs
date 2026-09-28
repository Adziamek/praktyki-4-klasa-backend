namespace Warehouse.Application.DTO.CustomerOrder;

public class CustomerOrderDto
{
    public int UserId { get; init; }
    public List<OrderItemDto> Items { get; init; } = new();
}