using DotNetEnv;
using StockTradingApplication.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

if (builder.Environment.IsDevelopment())
{
    Env.Load();
}

// For Database Extension
builder.Services.AddDatabaseServices();

var redisConnection = Environment.GetEnvironmentVariable("REDIS_CONNECTION");

// Register Services via Extensions
builder.Services.AddApplicationServices(builder.Configuration, redisConnection!);

// Exception Handlers Extension
builder.Services.AddExceptionHandlers();

// Automapper Extension
builder.Services.AddCustomAutoMapper();

// Custom Health Check Extension
builder.Services.AddCustomHealthChecks(redisConnection!);

// For scheduled job Extension
builder.Services.AddQuartzJobs();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Ensure tables are created -- Code First approach.
app.Services.EnsureDatabaseCreated();

//Middleware for global exception handling and Rate Limiting Extension .
app.UseCustomMiddlewares();

//Health check 
app.UseCustomHealthChecks();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();