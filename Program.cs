using DotNetEnv;
using StockTradingApplication.Configuration;
using StockTradingApplication.ExceptionHandlers.Handlers;
using StockTradingApplication.Extensions;
using StockTradingApplication.Middleware;
using StockTradingApplication.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

if (builder.Environment.IsDevelopment())
{
    Env.Load();
}

//For Database
builder.Services.AddDatabaseServices();

builder.Services.Configure<StockClientOptions>(builder.Configuration.GetSection("StockClient"));
builder.Services.AddHttpClient<StockService>();

builder.Services.AddSingleton<ExceptionResponseFactory>();
builder.Services.AddTransient<IStockService, StockService>();


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