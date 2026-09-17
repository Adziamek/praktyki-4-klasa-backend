namespace Warehouse.Application.DTO;

public  class ProductAddDto
{
    public string Name { get; init; } = string.Empty;
    public string Ean { get; init; } = string.Empty;
    public int CategoryId { get; init; }
}