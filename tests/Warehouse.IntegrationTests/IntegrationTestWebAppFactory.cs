using DotNet.Testcontainers.Builders;
using Microsoft.AspNetCore.Mvc.Testing;
using Testcontainers.PostgreSql;

namespace Warehouse.IntegrationTests;

public class IntegrationTestWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithImage("postgres:16-alpine")
        .WithDatabase("warehouse_test")
        .WithUsername("test")
        .WithPassword("test")
        .Build();

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();

        Environment.SetEnvironmentVariable(
            "ConnectionStrings__WarehouseDb",
            _dbContainer.GetConnectionString());

        Environment.SetEnvironmentVariable("Jwt__Key", "test-integration-secret-key-1234567890");
        Environment.SetEnvironmentVariable("Jwt__Issuer", "test-issuer");
        Environment.SetEnvironmentVariable("Jwt__Audience", "test-audience");
        Environment.SetEnvironmentVariable("Admin__Username", "admin");
        Environment.SetEnvironmentVariable("Admin__Password", "Test123!");
    }

    Task IAsyncLifetime.DisposeAsync() => _dbContainer.DisposeAsync().AsTask();
}