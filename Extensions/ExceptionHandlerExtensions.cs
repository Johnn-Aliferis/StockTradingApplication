using StockTradingApplication.ExceptionHandlers.Handlers;

namespace StockTradingApplication.Extensions;

public static class ExceptionHandlerExtensions
{
    public static void AddExceptionHandlers(this IServiceCollection services)
    {
        services.AddSingleton<ExceptionResponseFactory>();
        services.AddSingleton<ValidationExceptionHandler>();
        services.AddSingleton<StockClientExceptionHandler>();
        services.AddSingleton<PortfolioExceptionHandler>();
        services.AddSingleton<PortfolioTransactionExceptionHandler>();
        services.AddSingleton<GeneralExceptionHandler>();
    }
}