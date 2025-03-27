using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace StockTradingApplication.Health;

public class CustomHealthCheck : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
    {
        return Task.FromResult(HealthCheckResult.Healthy("Application is running fine"));
    }
}