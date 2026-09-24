namespace Warehouse.Domain.Entities;

public class ReturnRequest
{
    public int Id { get; set; }

    public int OrderId { get; set; }
    public CustomerOrder Order { get; set; } = null!;

    public string Status { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    public ICollection<ReturnItem> Items { get; set; } = new List<ReturnItem>();
}