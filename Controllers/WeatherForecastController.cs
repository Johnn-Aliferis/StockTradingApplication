using Microsoft.AspNetCore.Mvc;
using StockTradingApplication.DTOs;
using StockTradingApplication.Services;

namespace StockTradingApplication.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WeatherForecastController(ILogger<WeatherForecastController> logger, IStockDbService stockDbService) : ControllerBase
{
    [HttpGet("stock-data/{symbol}", Name ="stock-data-route")]
    public Task<StockDataDto?> GetHelloWorld(string symbol)
    {
        return stockDbService.GetStockAsync(symbol);
    }
}