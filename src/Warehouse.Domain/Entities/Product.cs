namespace Warehouse.Domain.Entities;

public class Product
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Ean { get; private set; } = string.Empty;
    public int CategoryId { get; set; }

    private Product()
    {
    }

    public Product(string name, string ean)
    {
        Id = Guid.NewGuid();
        Name = name;
        Ean = ean;
    }
    public Category Category { get; set; } = null!;

}