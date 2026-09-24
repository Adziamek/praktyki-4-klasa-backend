namespace Warehouse.Domain.Entities;

public class CustomerOrder
{
    public int Id { get; set; }

    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
    public ICollection<ReturnRequest> Returns { get; set; } = new List<ReturnRequest>();
}