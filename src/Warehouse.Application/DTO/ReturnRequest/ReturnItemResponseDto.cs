namespace Warehouse.Application.DTO.ReturnRequest;

public class ReturnItemResponseDto
{
    public int Id { get; init; }

    public int OrderItemId { get; init; }

    public int ProductId { get; init; }

    public int Quantity { get; init; }
}