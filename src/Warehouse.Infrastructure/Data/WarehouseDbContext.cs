using Microsoft.EntityFrameworkCore;
using Warehouse.Domain.Entities;

namespace Warehouse.Infrastructure.Data;

public class WarehouseDbContext : DbContext
{
    public WarehouseDbContext(DbContextOptions<WarehouseDbContext> options)
        : base(options)
    {
    }

    public DbSet<Product> Products => Set<Product>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Location> Locations => Set<Location>();
    public DbSet<CWarehouse> Warehouses => Set<CWarehouse>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Brand> Brands => Set<Brand>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Product
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(p => p.Id);

            entity.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(p => p.Ean)
                .IsRequired()
                .HasMaxLength(13);
        });

        // User
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);

            entity.Property(u => u.Id)
                .ValueGeneratedOnAdd();

            entity.Property(u => u.Username)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(u => u.PasswordHash)
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(u => u.Role)
                .IsRequired()
                .HasConversion<string>();

            entity.Property(u => u.CreatedAt)
                .IsRequired();

            entity.HasIndex(u => u.Username)
                .IsUnique();

            entity.HasIndex(u => u.Email)
                .IsUnique();
        });

        // Warehouse
        modelBuilder.Entity<CWarehouse>(entity =>
        {
            entity.HasKey(w => w.Id);

            entity.Property(w => w.Id)
                .ValueGeneratedOnAdd();

            entity.Property(w => w.Code)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(w => w.Name)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(w => w.Description)
                .HasMaxLength(500);

            entity.HasIndex(w => w.Code)
                .IsUnique();
        });

        // Location
        modelBuilder.Entity<Location>(entity =>
        {
            entity.HasKey(l => l.Id);

            entity.Property(l => l.Code)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(l => l.Name)
                .IsRequired()
                .HasMaxLength(200);

            entity.HasIndex(l => new { l.WarehouseId, l.Code })
                .IsUnique();

            entity.HasOne(l => l.Warehouse)
                .WithMany(w => w.Locations)
                .HasForeignKey(l => l.WarehouseId);
        });
        // Seed data - Warehouses
        modelBuilder.Entity<CWarehouse>().HasData(
            new CWarehouse
            {
                Id = 1,
                Code = "WH-01",
                Name = "Magazyn Główny Warszawa",
                Description = "Główny magazyn centralny",
                IsActive = true
            },
            new CWarehouse
            {
                Id = 2,
                Code = "WH-02",
                Name = "Magazyn Wrocław",
                Description = "Magazyn regionalny",
                IsActive = true
            }
        );

        // Seed data - Locations (przypisane do prawdziwych WarehouseId powyżej)
        modelBuilder.Entity<Location>().HasData(
            new Location
            {
                Id = 1,
                WarehouseId = 1,
                Code = "A-001",
                Name = "Regał A1",
                IsActive = true
            },
            new Location
            {
                Id = 2,
                WarehouseId = 1,
                Code = "A-002",
                Name = "Regał A2",
                IsActive = true
            },
            new Location
            {
                Id = 3,
                WarehouseId = 2,
                Code = "B-001",
                Name = "Regał B1",
                IsActive = true
            }
        );

        // Seed data - Categories (wymagane jako FK dla Product)
        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Elektronika", Description = "Sprzęt elektroniczny" },
            new Category { Id = 2, Name = "AGD", Description = "Sprzęt gospodarstwa domowego" },
            new Category { Id = 3, Name = "Akcesoria", Description = "Akcesoria komputerowe i inne" }
        );

        // Seed data - Brands (wymagane jako FK dla Product)
        modelBuilder.Entity<Brand>().HasData(
            new Brand { Id = 1, Name = "Samsung", Description = "Producent elektroniki" },
            new Brand { Id = 2, Name = "Bosch", Description = "Producent AGD i narzędzi" },
            new Brand { Id = 3, Name = "Logitech", Description = "Producent akcesoriów komputerowych" }
        );

        // Seed data - Products (Id jako stałe Guidy - wymagane dla powtarzalnych migracji)
        modelBuilder.Entity<Product>().HasData(
            new
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111101"),
                Name = "Samsung Galaxy Buds",
                Ean = "5901234123457",
                CategoryId = 1,
                BrandId = 1
            },
            new
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111102"),
                Name = "Samsung Smart TV 50\"",
                Ean = "5901234123458",
                CategoryId = 1,
                BrandId = 1
            },
            new
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111103"),
                Name = "Bosch Robot kuchenny",
                Ean = "5901234123459",
                CategoryId = 2,
                BrandId = 2
            },
            new
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111104"),
                Name = "Bosch Ekspres do kawy",
                Ean = "5901234123460",
                CategoryId = 2,
                BrandId = 2
            },
            new
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111105"),
                Name = "Logitech Mysz MX Master",
                Ean = "5901234123461",
                CategoryId = 3,
                BrandId = 3
            },
            new
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111106"),
                Name = "Logitech Klawiatura K380",
                Ean = "5901234123462",
                CategoryId = 3,
                BrandId = 3
            },
            new
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111107"),
                Name = "Samsung Pralka EcoBubble",
                Ean = "5901234123463",
                CategoryId = 2,
                BrandId = 1
            },
            new
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111108"),
                Name = "Bosch Wiertarka udarowa",
                Ean = "5901234123464",
                CategoryId = 3,
                BrandId = 2
            },
            new
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111109"),
                Name = "Logitech Kamera internetowa C920",
                Ean = "5901234123465",
                CategoryId = 1,
                BrandId = 3
            },
            new
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111110"),
                Name = "Samsung Monitor 27\"",
                Ean = "5901234123466",
                CategoryId = 1,
                BrandId = 1
            }
        );
    }
}