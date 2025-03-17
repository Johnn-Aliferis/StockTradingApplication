using System.Text.Json;
using StockTradingApplication.Exceptions;

namespace StockTradingApplication.ExceptionHandlers.Handlers;

public class PortfolioExceptionHandler
{
    public async Task HandleResponseAsync(HttpContext context, Exception exception)
    {
        if (exception is PortfolioException portfolioException)
        {
            context.Response.StatusCode = (int)portfolioException.Status;
            var response = new { error = "Portfolio Exception occurred.", details = exception.Message };
            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}