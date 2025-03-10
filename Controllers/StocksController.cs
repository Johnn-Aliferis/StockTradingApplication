using Microsoft.AspNetCore.Mvc;
using StockTradingApplication.DTOs;
using StockTradingApplication.Services;

namespace StockTradingApplication.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StocksController(IStockDbService stockDbService, IStockHistoryService stockHistoryService) : ControllerBase
{
    [HttpGet]
    public async Task<List<StockDataDto>> GetAllStocks()
    {
        return await stockDbService.GetStocksAsync();
    }


    [HttpGet("{symbol}", Name = "GetStockBySymbol")]
    public async Task<StockDataDto?> GetStockBySymbol(string symbol)
    {
        return await stockDbService.GetStockAsync(symbol);
    }

    [HttpGet("{symbol}/history", Name = "GetStockHistory")]
    public async Task<List<StockHistoryDto>> GetStockHistory(string symbol)
    {
        return await stockHistoryService.GetStockHistoryAsync(symbol);
    }
}