using AspNetCoreRateLimit;
using StockTradingApplication.Middleware;

namespace StockTradingApplication.Extensions;

public static  class MiddlewareExtensions
{
    public static void UseCustomMiddlewares(this WebApplication app)
    {
        // Exception Handling Middleware
        app.UseMiddleware<GlobalExceptionMiddleware>();

        // Rate Limiting Middleware
        app.UseIpRateLimiting();
    }
}