namespace Warehouse.Domain.Entities;

public class CWarehouse
{
    public int Id { get; set; }

    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string City { get; set; } = string.Empty;

    public bool Active { get; set; }

    public ICollection<Location> Locations { get; set; } = new List<Location>();
}
