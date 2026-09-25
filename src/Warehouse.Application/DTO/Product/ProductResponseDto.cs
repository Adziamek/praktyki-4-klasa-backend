namespace Warehouse.Application.DTO.Product;

public class ProductResponseDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Ean { get; init; } = string.Empty;
    public int CategoryId { get; init; }
    public int BrandId { get; init; }
    public decimal Price { get; init; }
}