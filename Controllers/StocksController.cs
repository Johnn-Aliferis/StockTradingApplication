using Microsoft.AspNetCore.Mvc;
using StockTradingApplication.DTOs;
using StockTradingApplication.Services;

namespace StockTradingApplication.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StocksController(IStockDbService stockDbService, IStockHistoryService stockHistoryService) : ControllerBase
{
    [HttpGet]
    public Task<List<StockDataDto>> GetAllStocks()
    {
        return stockDbService.GetStocksAsync();
    }
    
    
    [HttpGet("{symbol}", Name ="GetStockBySymbol")]
    public Task<StockDataDto?> GetStockBySymbol(string symbol)
    {
        return stockDbService.GetStockAsync(symbol);
    }
    
    [HttpGet("{symbol}/history", Name ="GetStockHistory")]
    public Task<List<StockHistoryDto>> GetStockHistory(string symbol)
    {
        return stockHistoryService.GetStockHistoryAsync(symbol);
    }
}