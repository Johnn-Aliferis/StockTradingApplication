using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using StockTradingApplication.Health;

namespace StockTradingApplication.Extensions;

public static class HealthCheckExtensions
{
    public static void AddCustomHealthChecks(this IServiceCollection services, string redisConnection)
    {
        services.AddHealthChecks()
            .AddRedis(redisConnection!, name: "Redis", timeout: TimeSpan.FromSeconds(2))
            .AddCheck<CustomHealthCheck>("app_running");
    }
    
    public static void UseCustomHealthChecks(this WebApplication app)
    {
        app.UseHealthChecks("/health");
        app.UseHealthChecks("/health/detailed", new HealthCheckOptions
        {
            ResponseWriter = async (context, report) =>
            {
                var response = new
                {
                    status = report.Status.ToString(),
                    checks = report.Entries.Select(e => new
                    {
                        component = e.Key,
                        status = e.Value.Status.ToString(),
                        description = e.Value.Description ?? "N/A"
                    }),
                    timestamp = DateTime.UtcNow
                };

                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(response);
            }
        });
    }
}