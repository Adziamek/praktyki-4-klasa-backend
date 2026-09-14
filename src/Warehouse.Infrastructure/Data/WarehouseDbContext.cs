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
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

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
                .IsRequired();

            entity.Property(u => u.CreatedAt)
                .IsRequired();

            entity.HasIndex(u => u.Username)
                .IsUnique();

            entity.HasIndex(u => u.Email)
                .IsUnique();
        });
        modelBuilder.Entity<User>().HasData(
            new
            {
                Id = 1,
                Username = "Admin",
                PasswordHash = "xd", //bedzie hash kiedys
                Email = "admin@example.com",
                Role = true,
                CreatedAt = new DateTime(2026, 9, 14, 0, 0, 0, DateTimeKind.Utc)
            }
        );
    }
}