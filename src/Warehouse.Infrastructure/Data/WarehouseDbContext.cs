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
    public DbSet<CWarehouse> Warehouses => Set<CWarehouse>();
    public DbSet<Location> Locations => Set<Location>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Brand> Brands => Set<Brand>();
    public DbSet<Stock> Stocks => Set<Stock>();
    public DbSet<CustomerOrder> CustomerOrders => Set<CustomerOrder>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<ReturnRequest> ReturnRequests => Set<ReturnRequest>();
    public DbSet<ReturnItem> ReturnItems => Set<ReturnItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // =========================================================
        // USER
        // =========================================================

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);

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
                .HasConversion<string>()
                .HasMaxLength(50);

            entity.Property(u => u.CreatedAt)
                .IsRequired();

            entity.HasIndex(u => u.Username)
                .IsUnique();

            entity.HasIndex(u => u.Email)
                .IsUnique();
        });

        // =========================================================
        // WAREHOUSE
        // =========================================================

        modelBuilder.Entity<CWarehouse>(entity =>
        {
            entity.HasKey(w => w.Id);

            entity.Property(w => w.Code)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(w => w.Name)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(w => w.City)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(w => w.Active)
                .IsRequired();

            entity.HasIndex(w => w.Code)
                .IsUnique();
        });

        // =========================================================
        // LOCATION
        // =========================================================

        modelBuilder.Entity<Location>(entity =>
        {
            entity.HasKey(l => l.Id);

            entity.Property(l => l.Code)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(l => l.Name)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(l => l.Active)
                .IsRequired();

            entity.HasOne(l => l.CWarehouse)
                .WithMany(w => w.Locations)
                .HasForeignKey(l => l.WarehouseId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(l => new
            {
                l.WarehouseId,
                l.Code
            })
            .IsUnique();
        });

        // =========================================================
        // CATEGORY
        // =========================================================

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(c => c.Id);

            entity.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(c => c.Description)
                .HasMaxLength(1000);

            entity.HasIndex(c => c.Name)
                .IsUnique();
        });

        // =========================================================
        // BRAND
        // =========================================================

        modelBuilder.Entity<Brand>(entity =>
        {
            entity.HasKey(b => b.Id);

            entity.Property(b => b.Name)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(b => b.Description)
                .HasMaxLength(1000);

            entity.HasIndex(b => b.Name)
                .IsUnique();
        });

        // =========================================================
        // PRODUCT
        // =========================================================

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(p => p.Id);

            entity.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(p => p.Ean)
                .IsRequired()
                .HasMaxLength(13);

            entity.HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(p => p.Brand)
                .WithMany(b => b.Products)
                .HasForeignKey(p => p.BrandId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(p => p.Ean)
                .IsUnique();
        });

        // =========================================================
        // STOCK
        // =========================================================

        modelBuilder.Entity<Stock>(entity =>
        {
            entity.HasKey(s => s.Id);

            entity.Property(s => s.Quantity)
                .IsRequired();

            entity.HasOne(s => s.Product)
                .WithMany()
                .HasForeignKey(s => s.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(s => s.Location)
                .WithMany(l => l.Stocks)
                .HasForeignKey(s => s.LocationId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(s => new
            {
                s.ProductId,
                s.LocationId
            })
            .IsUnique();
        });

        // =========================================================
        // CUSTOMER ORDER
        // =========================================================

        modelBuilder.Entity<CustomerOrder>(entity =>
        {
            entity.HasKey(o => o.Id);

            entity.Property(o => o.Status)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(o => o.CreatedAt)
                .IsRequired();

            entity.HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(o => o.UserId);
            entity.HasIndex(o => o.Status);
            entity.HasIndex(o => o.CreatedAt);
        });

        // =========================================================
        // ORDER ITEM
        // =========================================================

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(oi => oi.Id);

            entity.Property(oi => oi.Quantity)
                .IsRequired();

            entity.Property(oi => oi.UnitPrice)
                .IsRequired();

            entity.HasOne(oi => oi.Order)
                .WithMany(o => o.Items)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(oi => oi.Product)
                .WithMany()
                .HasForeignKey(oi => oi.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(oi => oi.OrderId);
            entity.HasIndex(oi => oi.ProductId);
        });

        // =========================================================
        // RETURN REQUEST
        // =========================================================

        modelBuilder.Entity<ReturnRequest>(entity =>
        {
            entity.HasKey(r => r.Id);

            entity.Property(r => r.Status)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(r => r.Reason)
                .IsRequired()
                .HasMaxLength(1000);

            entity.Property(r => r.CreatedAt)
                .IsRequired();

            entity.HasOne(r => r.Order)
                .WithMany(o => o.Returns)
                .HasForeignKey(r => r.OrderId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(r => r.OrderId);
            entity.HasIndex(r => r.Status);
        });

        // =========================================================
        // RETURN ITEM
        // =========================================================

        modelBuilder.Entity<ReturnItem>(entity =>
        {
            entity.HasKey(ri => ri.Id);

            entity.Property(ri => ri.Quantity)
                .IsRequired();

            entity.HasOne(ri => ri.Return)
                .WithMany(r => r.Items)
                .HasForeignKey(ri => ri.ReturnId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(ri => ri.OrderItem)
                .WithMany(oi => oi.Returns)
                .HasForeignKey(ri => ri.OrderItemId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(ri => ri.ReturnId);
            entity.HasIndex(ri => ri.OrderItemId);
        });

        // =========================================================
        // SEED - WAREHOUSES
        // =========================================================

        modelBuilder.Entity<CWarehouse>().HasData(
            new CWarehouse
            {
                Id = 1,
                Code = "WH-01",
                Name = "Magazyn Główny Warszawa",
                City = "Warszawa",
                Active = true
            },
            new CWarehouse
            {
                Id = 2,
                Code = "WH-02",
                Name = "Magazyn Wrocław",
                City = "Wrocław",
                Active = true
            }
        );

        // =========================================================
        // SEED - LOCATIONS
        // =========================================================

        modelBuilder.Entity<Location>().HasData(
            new Location
            {
                Id = 1,
                WarehouseId = 1,
                Code = "A-001",
                Name = "Regał A1",
                Active = true
            },
            new Location
            {
                Id = 2,
                WarehouseId = 1,
                Code = "A-002",
                Name = "Regał A2",
                Active = true
            },
            new Location
            {
                Id = 3,
                WarehouseId = 2,
                Code = "B-001",
                Name = "Regał B1",
                Active = true
            }
        );

        // =========================================================
        // SEED - CATEGORIES
        // =========================================================

        modelBuilder.Entity<Category>().HasData(
            new Category
            {
                Id = 1,
                Name = "Elektronika",
                Description = "Sprzęt elektroniczny"
            },
            new Category
            {
                Id = 2,
                Name = "AGD",
                Description = "Sprzęt gospodarstwa domowego"
            },
            new Category
            {
                Id = 3,
                Name = "Akcesoria",
                Description = "Akcesoria komputerowe i inne"
            }
        );

        // =========================================================
        // SEED - BRANDS
        // =========================================================

        modelBuilder.Entity<Brand>().HasData(
            new Brand
            {
                Id = 1,
                Name = "Samsung",
                Description = "Producent elektroniki"
            },
            new Brand
            {
                Id = 2,
                Name = "Bosch",
                Description = "Producent AGD i narzędzi"
            },
            new Brand
            {
                Id = 3,
                Name = "Logitech",
                Description = "Producent akcesoriów komputerowych"
            }
        );

        // =========================================================
        // SEED - PRODUCTS
        // =========================================================

        modelBuilder.Entity<Product>().HasData(
            new
            {
                Id = 1,
                Name = "Samsung Galaxy Buds",
                Ean = "5901234123457",
                CategoryId = 1,
                BrandId = 1
            },
            new
            {
                Id = 2,
                Name = "Samsung Smart TV 50\"",
                Ean = "5901234123458",
                CategoryId = 1,
                BrandId = 1
            },
            new
            {
                Id = 3,
                Name = "Bosch Robot kuchenny",
                Ean = "5901234123459",
                CategoryId = 2,
                BrandId = 2
            },
            new
            {
                Id = 4,
                Name = "Bosch Ekspres do kawy",
                Ean = "5901234123460",
                CategoryId = 2,
                BrandId = 2
            },
            new
            {
                Id = 5,
                Name = "Logitech Mysz MX Master",
                Ean = "5901234123461",
                CategoryId = 3,
                BrandId = 3
            },
            new
            {
                Id = 6,
                Name = "Logitech Klawiatura K380",
                Ean = "5901234123462",
                CategoryId = 3,
                BrandId = 3
            },
            new
            {
                Id = 7,
                Name = "Samsung Pralka EcoBubble",
                Ean = "5901234123463",
                CategoryId = 2,
                BrandId = 1
            },
            new
            {
                Id = 8,
                Name = "Bosch Wiertarka udarowa",
                Ean = "5901234123464",
                CategoryId = 3,
                BrandId = 2
            },
            new
            {
                Id = 9,
                Name = "Logitech Kamera internetowa C920",
                Ean = "5901234123465",
                CategoryId = 1,
                BrandId = 3
            },
            new
            {
                Id = 10,
                Name = "Samsung Monitor 27\"",
                Ean = "5901234123466",
                CategoryId = 1,
                BrandId = 1
            }
        );
    }
}
