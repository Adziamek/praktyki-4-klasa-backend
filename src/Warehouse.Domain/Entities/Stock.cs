namespace Warehouse.Domain.Entities;

public class Stock
{
    public int Id { get; set; }

    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public int LocationId { get; set; }
    public Location Location { get; set; } = null!;

    public int Quantity { get; set; }
}