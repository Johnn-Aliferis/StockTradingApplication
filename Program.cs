using DotNetEnv;
using StockTradingApplication.Decorators;
using StockTradingApplication.Entities;
using StockTradingApplication.ExceptionHandlers.Handlers;
using StockTradingApplication.Exceptions;
using StockTradingApplication.Extensions;
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

// For Redis-Cache
builder.Services.AddStackExchangeRedisCache(options =>
{
    var redisConnection = Environment.GetEnvironmentVariable("REDIS_CONNECTION");
    options.Configuration = redisConnection;
});


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
builder.Services.AddTransient<ValidationService>();

builder.Services.Decorate<IStockRepository, StockRepositoryLoggingDecorator>();
builder.Services.Decorate<IStockDbService, StockDbServiceCachingDecorator>();

//Automapper 
builder.Services.AddAutoMapper(typeof(StockProfile));
builder.Services.AddAutoMapper(typeof(StockHistoryProfile));
builder.Services.AddAutoMapper(typeof(UserProfile));
builder.Services.AddAutoMapper(typeof(Portfolio));
builder.Services.AddAutoMapper(typeof(PortfolioHolding));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Ensure tables are created -- Code First approach.
app.Services.EnsureDatabaseCreated();

//Middleware for global exception handling.
app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();