using System.Text.Json;
using StockTradingApplication.Exceptions;

namespace StockTradingApplication.ExceptionHandlers.Handlers;

public class StockClientExceptionHandler
{
    public async Task HandleResponseAsync(HttpContext context, Exception exception)
    {
        if (exception is StockClientException stockClientException)
        {
            context.Response.StatusCode = (int)stockClientException.Status;
            var response = new { error = "Stock Client Exception occurred.", details = exception.Message };
            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}