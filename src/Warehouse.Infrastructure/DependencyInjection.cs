using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Warehouse.Application.Interfaces;
using Warehouse.Infrastructure.Data;
using Warehouse.Infrastructure.Services;

namespace Warehouse.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        string connectionString)
    {
        services.AddDbContext<WarehouseDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ICategoryService, CategoryService>();

        return services;
    }
}