namespace Warehouse.Application.DTO.CustomerOrder;

public class OrderItemResponseDto
{
    public int Id { get; init; }

    public int ProductId { get; init; }

    public int Quantity { get; init; }

    public decimal UnitPrice { get; init; }
}