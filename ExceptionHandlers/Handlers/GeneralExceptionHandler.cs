using System.Text.Json;
using StockTradingApplication.Exceptions;

namespace StockTradingApplication.ExceptionHandlers.Handlers;

public class GeneralExceptionHandler
{
    public async Task HandleResponseAsync(HttpContext context, Exception exception)
    {
        if (exception is GeneralException generalException)
        {
            context.Response.StatusCode = (int)generalException.Status;
            var response = new { error = "General Exception occurred.", details = exception.Message };
            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}