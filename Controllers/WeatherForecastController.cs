using Microsoft.AspNetCore.Mvc;

namespace StockTradingApplication.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WeatherForecastController(ILogger<WeatherForecastController> logger) : ControllerBase
{
    [HttpGet("hello-world",Name = "hello-world-route")]
    public Task<string> GetHelloWorld()
    {
        return  Task.FromResult("Hello World");
    }
}