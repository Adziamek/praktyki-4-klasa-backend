using Microsoft.EntityFrameworkCore;
using Warehouse.Domain.Entities;

namespace Warehouse.Infrastructure.Data;

public static class ModelBuilderExtensions
{
    public static void Seed(this ModelBuilder modelBuilder)
    {
        // 1. Seed Kategorii
        modelBuilder.Entity<Category>().HasData(
            new Category
            {
                Id = 1,
                Name = "Elektronika",
                Description = "Sprzęt elektroniczny i akcesoria"
            },
            new Category
            {
                Id = 2,
                Name = "Materiały Biurowe",
                Description = "Artykuły papiernicze i wyposażenie biura"
            },
            new Category
            {
                Id = 3,
                Name = "Narzędzia",
                Description = "Narzędzia ręczne i elektronarzędzia"
            }
        );

        // 2. Seed Produktów przypisanych do kategorii
        modelBuilder.Entity<Product>().HasData(
            new
            {
                Id = Guid.Parse("a1111111-1111-1111-1111-111111111111"),
                Name = "Laptop Dell XPS 15",
                Ean = "5901234567890",
                CategoryId = 1
            },
            new
            {
                Id = Guid.Parse("a2222222-2222-2222-2222-222222222222"),
                Name = "Mysz Bezprzewodowa Logitech MX Master 3S",
                Ean = "5901234567891",
                CategoryId = 1
            },
            new
            {
                Id = Guid.Parse("b1111111-1111-1111-1111-111111111111"),
                Name = "Papier A4 PolSpeed 80g (Karton 5 ryz)",
                Ean = "5901234567892",
                CategoryId = 2
            },
            new
            {
                Id = Guid.Parse("b2222222-2222-2222-2222-222222222222"),
                Name = "Długopis Cienkopis Czarny Pilot",
                Ean = "5901234567893",
                CategoryId = 2
            },
            new
            {
                Id = Guid.Parse("c1111111-1111-1111-1111-111111111111"),
                Name = "Wkrętarka Akumulatorowa Bosch Professional",
                Ean = "5901234567894",
                CategoryId = 3
            }
        );
    }
}