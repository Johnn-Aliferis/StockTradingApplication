using AspNetCoreRateLimit;
using DotNetEnv;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;
using StockTradingApplication.Decorators;
using StockTradingApplication.Options;
using StockTradingApplication.Repository.Implementations;
using StockTradingApplication.Repository.Interfaces;
using StockTradingApplication.Services;
using StockTradingApplication.Services.Implementations;
using StockTradingApplication.Services.Interfaces;

namespace StockTradingApplication.Extensions;

public static class ServiceExtensions
{
    public static void AddApplicationServices(this IServiceCollection services, IConfiguration configuration, string redisConnection)
    {
        // Load Environment Variables
        Env.Load();

        // Add Redis Caching
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = redisConnection;
        });

        // Rate Limiting
        services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConnection));
        services.Configure<IpRateLimitOptions>(configuration.GetSection("IpRateLimiting"));
        services.Configure<IpRateLimitPolicies>(configuration.GetSection("IpRateLimitPolicies"));
        services.AddMemoryCache();
        services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
        services.AddSingleton<IIpPolicyStore, DistributedCacheIpPolicyStore>();
        services.AddSingleton<IRateLimitCounterStore, DistributedCacheRateLimitCounterStore>();
        services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();
        services.AddSingleton<IDistributedCache, Microsoft.Extensions.Caching.StackExchangeRedis.RedisCache>();

        // Options Pattern
        services.Configure<StockClientOptions>(configuration.GetSection("StockClient"));
        services.AddHttpClient<ExternalStockService>();

        // Core Services
        services.AddTransient<IExternalStockService, ExternalStockService>();
        services.AddTransient<IStockDbService, StockDbService>();
        services.AddTransient<IStockRepository, StockRepository>();
        services.AddTransient<IStockHistoryRepository, StockHistoryRepository>();
        services.AddTransient<IStockHistoryService, StockHistoryService>();
        services.AddTransient<IUserService, UserService>();
        services.AddTransient<IUserRepository, UserRepository>();
        services.AddTransient<IPortfolioService, PortfolioService>();
        services.AddTransient<IPortfolioRepository, PortfolioRepository>();
        services.AddTransient<IPortfolioTransactionService, PortfolioTransactionService>();
        services.AddTransient<IPortfolioTransactionRepository, PortfolioTransactionRepository>();
        services.AddTransient<IUnitOfWork, UnitOfWork>();
        services.AddTransient<ValidationService>();

        // Decorators
        services.Decorate<IStockRepository, StockRepositoryLoggingDecorator>();
        services.Decorate<IStockDbService, StockDbServiceCachingDecorator>();
    }
}