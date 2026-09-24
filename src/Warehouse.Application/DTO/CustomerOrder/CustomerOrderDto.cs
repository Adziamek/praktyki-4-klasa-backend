namespace Warehouse.Application.DTO.CustomerOrder;

public class CustomerOrderDto
{
    public int UserId { get; init; }
    public string Status { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
}