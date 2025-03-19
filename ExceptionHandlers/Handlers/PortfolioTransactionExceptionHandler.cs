using System.Text.Json;
using StockTradingApplication.Exceptions;

namespace StockTradingApplication.ExceptionHandlers.Handlers;

public class PortfolioTransactionExceptionHandler
{
    public async Task HandleResponseAsync(HttpContext context, Exception exception)
    {
        if (exception is PortfolioTransactionException portfolioTransactionException)
        {
            context.Response.StatusCode = (int)portfolioTransactionException.Status;
            var response = new { error = "Portfolio Transaction Exception occurred.", details = exception.Message };
            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}