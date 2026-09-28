namespace Warehouse.Application.DTO.Product;

public class ProductResponseDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Ean { get; init; } = string.Empty;
    public int CategoryId { get; init; }
    public string Category { get; set; } = string.Empty;
    public int BrandId { get; init; }
    public string Brand { get; set; } = string.Empty;

    public decimal Price { get; init; }
    public int Quantity { get; set; }

    public List<ProductLocationDto> Locations { get; init; } = [];
}