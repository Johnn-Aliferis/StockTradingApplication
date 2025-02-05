using System.Text.Json;
using StockTradingApplication.Exceptions;

namespace StockTradingApplication.ExceptionHandlers.Handlers;

public class ExceptionResponseFactory
{
    private readonly Dictionary<Type, object> _handlers = new Dictionary<Type, object>()
    {
        { typeof(ValidationException), new ValidationExceptionHandler() },
        { typeof(StockClientException), new StockClientExceptionHandler() },
        { typeof(GeneralException), new GeneralExceptionHandler() }
    };

    public async Task HandleResponseAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        var handler = _handlers[exception.GetType()];
        switch (handler)
        {
            case ValidationExceptionHandler validationHandler:
                await validationHandler.HandleResponseAsync(context, (ValidationException)exception);
                break;

            case StockClientExceptionHandler stockHandler:
                await stockHandler.HandleResponseAsync(context, (StockClientException)exception);
                break;

            case GeneralExceptionHandler generalHandler:
                await generalHandler.HandleResponseAsync(context, (GeneralException)exception);
                break;

            default:
                context.Response.StatusCode = 500;
                var response = new { error = "An unexpected error occurred." };
                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
                break;
        }
    }
}