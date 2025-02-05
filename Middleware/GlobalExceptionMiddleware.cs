using StockTradingApplication.ExceptionHandlers.Handlers;

namespace StockTradingApplication.Middleware;

public class GlobalExceptionMiddleware(
    RequestDelegate next,
    ILogger<GlobalExceptionMiddleware> logger,
    ExceptionResponseFactory exceptionResponseFactory)

{
    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await next(httpContext);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unhandled exception occurred: {Message}", ex.Message);
            await exceptionResponseFactory.HandleResponseAsync(httpContext, ex);
        }
    }
}