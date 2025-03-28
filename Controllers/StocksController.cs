using Microsoft.AspNetCore.Mvc;
using StockTradingApplication.DTOs;
using StockTradingApplication.Services.Interfaces;

namespace StockTradingApplication.Controllers;

[ApiController]
[Route("api/v1/stocks")]
public class StocksController(IStockDbService stockDbService, IStockHistoryService stockHistoryService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<StockDataDto>>> GetAllStocks()
    {
        var stocks = await stockDbService.GetStocksAsync();
        return Ok(stocks);
    }


    [HttpGet("{symbol}", Name = "GetStockBySymbol")]
    public async Task<ActionResult<StockDataDto?>> GetStockBySymbol(string symbol)
    {
        var stock = await stockDbService.GetStockAsync(symbol);
        return Ok(stock);
    }

    [HttpGet("{symbol}/history", Name = "GetStockHistory")]
    public async Task<ActionResult<List<StockHistoryDto>>> GetStockHistory(string symbol)
    {
        var histories = await stockHistoryService.GetStockHistoryAsync(symbol);
        return Ok(histories);
    }
}