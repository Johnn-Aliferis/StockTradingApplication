using StockTradingApplication.Configuration;
using StockTradingApplication.ExceptionHandlers.Handlers;
using StockTradingApplication.Middleware;
using StockTradingApplication.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

//Middleware for global exception handling.
app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();