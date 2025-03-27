using AspNetCoreRateLimit;
using DotNetEnv;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;
using StockTradingApplication.Decorators;
using StockTradingApplication.Entities;
using StockTradingApplication.ExceptionHandlers.Handlers;
using StockTradingApplication.Extensions;
using StockTradingApplication.Health;
using StockTradingApplication.Middleware;
using StockTradingApplication.Options;
using StockTradingApplication.Profiles;
using StockTradingApplication.Repository.Implementations;
using StockTradingApplication.Repository.Interfaces;
using StockTradingApplication.Services;
using StockTradingApplication.Services.Implementations;
using StockTradingApplication.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

if (builder.Environment.IsDevelopment())
{
    Env.Load();
}

// For Database
builder.Services.AddDatabaseServices();

var redisConnection = Environment.GetEnvironmentVariable("REDIS_CONNECTION");

// For Redis-Cache
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = redisConnection;
});

// Rate limiting 
builder.Services.AddSingleton<IConnectionMultiplexer>(
    ConnectionMultiplexer.Connect(redisConnection!)
);

builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
builder.Services.Configure<IpRateLimitPolicies>(builder.Configuration.GetSection("IpRateLimitPolicies"));

builder.Services.AddMemoryCache();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
builder.Services.AddSingleton<IIpPolicyStore, DistributedCacheIpPolicyStore>();
builder.Services.AddSingleton<IRateLimitCounterStore, DistributedCacheRateLimitCounterStore>();
builder.Services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();
builder.Services.AddSingleton<IDistributedCache, Microsoft.Extensions.Caching.StackExchangeRedis.RedisCache>();


// For scheduled job
builder.Services.AddQuartzJobs();

// Options pattern 
builder.Services.Configure<StockClientOptions>(builder.Configuration.GetSection("StockClient"));
builder.Services.AddHttpClient<ExternalStockService>();

//Custom Exceptions
builder.Services.AddSingleton<ExceptionResponseFactory>();
builder.Services.AddSingleton<ValidationExceptionHandler>();
builder.Services.AddSingleton<StockClientExceptionHandler>();
builder.Services.AddSingleton<PortfolioExceptionHandler>();
builder.Services.AddSingleton<PortfolioTransactionExceptionHandler>();
builder.Services.AddSingleton<GeneralExceptionHandler>();

builder.Services.AddTransient<IExternalStockService, ExternalStockService>();
builder.Services.AddTransient<IStockDbService, StockDbService>();
builder.Services.AddTransient<IStockRepository, StockRepository>();
builder.Services.AddTransient<IStockHistoryRepository, StockHistoryRepository>();
builder.Services.AddTransient<IStockHistoryService, StockHistoryService>();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IUserRepository, UserRepository>();
builder.Services.AddTransient<IPortfolioService, PortfolioService>();
builder.Services.AddTransient<IPortfolioRepository, PortfolioRepository>();
builder.Services.AddTransient<IPortfolioTransactionService, PortfolioTransactionService>();
builder.Services.AddTransient<IPortfolioTransactionRepository, PortfolioTransactionRepository>();
builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();
builder.Services.AddTransient<ValidationService>();

builder.Services.Decorate<IStockRepository, StockRepositoryLoggingDecorator>();
builder.Services.Decorate<IStockDbService, StockDbServiceCachingDecorator>();

//Automapper 
builder.Services.AddAutoMapper(typeof(StockProfile));
builder.Services.AddAutoMapper(typeof(StockHistoryProfile));
builder.Services.AddAutoMapper(typeof(UserProfile));
builder.Services.AddAutoMapper(typeof(Portfolio));
builder.Services.AddAutoMapper(typeof(PortfolioHolding));
builder.Services.AddAutoMapper(typeof(PortfolioTransaction));

// Health Checks
builder.Services.AddHealthChecks()
    .AddRedis(
        redisConnection!,
        name: "Redis",
        timeout: TimeSpan.FromSeconds(2)
    )
    .AddCheck<CustomHealthCheck>("app_running");

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Ensure tables are created -- Code First approach.
app.Services.EnsureDatabaseCreated();

//Health check 
app.UseHealthChecks("/health");
app.UseHealthChecks("/health/detailed", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        var response = new
        {
            status = report.Status.ToString(),
            checks = report.Entries.Select(e => new
            {
                component = e.Key,
                status = e.Value.Status.ToString(),
                description = e.Value.Description ?? "N/A"
            }),
            timestamp = DateTime.UtcNow
        };

        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(response);
    }
});

//Middleware for global exception handling.
app.UseMiddleware<GlobalExceptionMiddleware>();

//Rate Limiter Middleware
app.UseIpRateLimiting();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();