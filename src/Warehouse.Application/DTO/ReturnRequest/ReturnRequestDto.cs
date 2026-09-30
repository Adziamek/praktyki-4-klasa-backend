namespace Warehouse.Application.DTO.ReturnRequest;

public class ReturnRequestDto
{
    public int OrderId { get; init; }

    public string Reason { get; init; } = string.Empty;

    public List<ReturnItemDto> Items { get; init; } = new();
}