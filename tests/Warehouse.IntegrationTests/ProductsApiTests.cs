using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using Warehouse.Application.DTO.Product;

namespace Warehouse.IntegrationTests;

public class ProductsApiTests : IClassFixture<IntegrationTestWebAppFactory>
{
    private readonly HttpClient _client;

    public ProductsApiTests(IntegrationTestWebAppFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetProducts_Returns_SeededData()
    {
        var response = await _client.GetAsync("/api/products");
        response.EnsureSuccessStatusCode();

        var products = await response.Content.ReadFromJsonAsync<List<ProductResponseDto>>();

        products.Should().NotBeNull();
        products!.Count.Should().BeGreaterThanOrEqualTo(10);
    }

    [Fact]
    public async Task AddProduct_Then_GetById_Returns_CreatedProduct()
    {
        var newProduct = new ProductDto
        {
            Name = "Testowa klawiatura",
            Ean = "1234567890199",
            CategoryId = 3,
            BrandId = 3
        };

        var postResponse = await _client.PostAsJsonAsync("/api/products", newProduct);
        postResponse.EnsureSuccessStatusCode();

        var created = await postResponse.Content.ReadFromJsonAsync<ProductResponseDto>();
        created!.Name.Should().Be(newProduct.Name);

        var getResponse = await _client.GetAsync($"/api/products/{created.Id}");
        getResponse.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task AddProduct_With_Duplicate_Ean_Returns_Conflict()
    {
        var product = new ProductDto
        {
            Name = "Pierwszy produkt",
            Ean = "9999999999999",
            CategoryId = 3,
            BrandId = 3
        };

        var firstResponse = await _client.PostAsJsonAsync("/api/products", product);
        firstResponse.EnsureSuccessStatusCode();

        var duplicate = new ProductDto
        {
            Name = "Inna nazwa, to samo EAN",
            Ean = "9999999999999", // ten sam EAN
            CategoryId = 3,
            BrandId = 3
        };

        var secondResponse = await _client.PostAsJsonAsync("/api/products", duplicate);

        secondResponse.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task AddProduct_With_NonExistent_Category_Returns_NotFound()
    {
        var product = new ProductDto
        {
            Name = "Produkt z nieistniejącą kategorią",
            Ean = "8888888888888",
            CategoryId = 999, // nie istnieje w seedzie
            BrandId = 3
        };

        var response = await _client.PostAsJsonAsync("/api/products", product);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}