namespace Warehouse.Domain.Entities;

public class ReturnItem
{
    public int Id { get; set; }

    public int ReturnId { get; set; }
    public ReturnRequest Return { get; set; } = null!;

    public int OrderItemId { get; set; }
    public OrderItem OrderItem { get; set; } = null!;

    public int Quantity { get; set; }
}