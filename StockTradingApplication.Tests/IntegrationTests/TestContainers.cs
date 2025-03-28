using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using StockTradingApplication.Persistence;
using Testcontainers.PostgreSql;

namespace StockTradingApplication.Tests.IntegrationTests;

public class TestContainers : IAsyncLifetime
{
    public required AppDbContext DbContext { get; set; }
    private readonly PostgreSqlContainer _dbContainer;

    public TestContainers()
    {
        _dbContainer = new PostgreSqlBuilder()
            .WithDatabase("testdb")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .Build();
    }

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();

        var services = new ServiceCollection();
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(_dbContainer.GetConnectionString()));

        var serviceProvider = services.BuildServiceProvider();
        DbContext = serviceProvider.GetRequiredService<AppDbContext>();
        
        await DbContext.Database.EnsureCreatedAsync();
        
        // await DbContext.Database.MigrateAsync();
    }

    public async Task DisposeAsync()
    {
        await _dbContainer.StopAsync();
    }
}