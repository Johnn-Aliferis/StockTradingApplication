using StockTradingApplication.Entities;
using StockTradingApplication.Profiles;

namespace StockTradingApplication.Extensions;

public static class AutoMapperExtensions
{
    public static void AddCustomAutoMapper(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(StockProfile));
        services.AddAutoMapper(typeof(StockHistoryProfile));
        services.AddAutoMapper(typeof(UserProfile));
        services.AddAutoMapper(typeof(Portfolio));
        services.AddAutoMapper(typeof(PortfolioHolding));
        services.AddAutoMapper(typeof(PortfolioTransaction));
    }
}