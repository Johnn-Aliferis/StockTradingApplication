using System.Text.Json;
using StockTradingApplication.Exceptions;

namespace StockTradingApplication.ExceptionHandlers.Handlers;

public class ValidationExceptionHandler
{
    public async Task HandleResponseAsync(HttpContext context, Exception exception)
    {
        if (exception is ValidationException validationEx)
        {
            context.Response.StatusCode = (int)validationEx.Status;
            var response = new { error = "ValidationException occurred.", details = exception.Message };
            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}