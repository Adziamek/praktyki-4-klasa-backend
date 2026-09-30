namespace Warehouse.Application.DTO.ReturnRequest;

public class ReturnRequestResponseDto
{
    public int Id { get; init; }

    public int OrderId { get; init; }

    public string Status { get; init; } = string.Empty;

    public string Reason { get; init; } = string.Empty;

    public DateTime CreatedAt { get; init; }

    public List<ReturnItemResponseDto> Items { get; init; } = new();
}