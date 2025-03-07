using DotNetEnv;
using StockTradingApplication.Configuration;
using StockTradingApplication.Decorators;
using StockTradingApplication.ExceptionHandlers.Handlers;
using StockTradingApplication.Extensions;
using StockTradingApplication.Middleware;
using StockTradingApplication.Repository;
using StockTradingApplication.Services;

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

builder.Services.AddSingleton<ExceptionResponseFactory>();
builder.Services.AddSingleton<ValidationExceptionHandler>();
builder.Services.AddSingleton<StockClientExceptionHandler>();
builder.Services.AddSingleton<GeneralExceptionHandler>();
builder.Services.AddTransient<IExternalStockService, ExternalStockService>();
builder.Services.AddTransient<IStockDbService, StockDbService>();
builder.Services.AddTransient<IStockRepository, StockRepository>();
builder.Services.AddTransient<IStockHistoryRepository, StockHistoryRepository>();
builder.Services.AddTransient<IStockHistoryService, StockHistoryService>();
builder.Services.Decorate<IStockRepository, StockRepositoryLoggingDecorator>();
builder.Services.Decorate<IStockDbService, StockDbServiceCachingDecorator>();

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