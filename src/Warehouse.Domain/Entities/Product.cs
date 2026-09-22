namespace Warehouse.Domain.Entities;

public class Product
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Ean { get; set; } = string.Empty;
    public int CategoryId { get; set; }
    public int BrandId { get; set; }
}