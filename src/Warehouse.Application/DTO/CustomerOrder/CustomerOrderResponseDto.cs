namespace Warehouse.Application.DTO.CustomerOrder;

public class CustomerOrderResponseDto
{
    public int Id { get; init; }
    public int UserId { get; init; }
    public string Status { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
}