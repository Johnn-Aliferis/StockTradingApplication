using System.Text.Json;
using StockTradingApplication.Exceptions;

namespace StockTradingApplication.ExceptionHandlers.Handlers;

public class ExceptionResponseFactory(
    ValidationExceptionHandler validationHandler,
    StockClientExceptionHandler stockHandler,
    PortfolioExceptionHandler portfolioHandler,
    PortfolioTransactionExceptionHandler portfolioTransactionHandler,
    GeneralExceptionHandler generalHandler)
{
    private readonly Dictionary<Type, object> _handlers = new Dictionary<Type, object>()
    {
        { typeof(ValidationException), validationHandler },
        { typeof(StockClientException), stockHandler },
        { typeof(PortfolioException), portfolioHandler },
        { typeof(PortfolioTransactionException), portfolioTransactionHandler },
        { typeof(GeneralException), generalHandler }
    };

    public async Task HandleResponseAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        var handler = _handlers.ContainsKey(exception.GetType()) ? _handlers[exception.GetType()] : null;
        switch (handler)
        {
            case ValidationExceptionHandler validationHandler:
                await validationHandler.HandleResponseAsync(context, (ValidationException)exception);
                break;

            case StockClientExceptionHandler stockHandler:
                await stockHandler.HandleResponseAsync(context, (StockClientException)exception);
                break;

            case PortfolioExceptionHandler portfolioExceptionHandler:
                await portfolioExceptionHandler.HandleResponseAsync(context, (PortfolioException)exception);
                break;

            case PortfolioTransactionExceptionHandler portfolioTransactionHandler:
                await portfolioTransactionHandler.HandleResponseAsync(context, (PortfolioTransactionException)exception);
                break;

            case GeneralExceptionHandler generalHandler:
                await generalHandler.HandleResponseAsync(context, (GeneralException)exception);
                break;

            default:
                context.Response.StatusCode = 500;
                var response = new
                    { error = $"An Exception of {exception.GetType()} occurred.", details = exception.Message };
                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
                break;
        }
    }
}