using Microsoft.EntityFrameworkCore;
using StockTradingApplication.Persistence;

namespace StockTradingApplication.Extensions;

public static class DatabaseExtensions
{
    public static void EnsureDatabaseCreated(this IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        dbContext.Database.EnsureCreated();
    }
    
    public static IServiceCollection AddDatabaseServices(this IServiceCollection services)
    {
        var connectionString = $"Host={Environment.GetEnvironmentVariable("DB_HOST")};" +
                               $"Database={Environment.GetEnvironmentVariable("POSTGRES_DB")};" +
                               $"Username={Environment.GetEnvironmentVariable("POSTGRES_USER")};" +
                               $"Password={Environment.GetEnvironmentVariable("POSTGRES_PASSWORD")}";

        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString).UseLazyLoadingProxies());
        
        services.AddHealthChecks()
            .AddNpgSql(
                connectionString,
                name: "postgresql",
                timeout: TimeSpan.FromSeconds(2)
            );

        return services;
    }
}