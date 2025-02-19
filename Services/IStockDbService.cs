using StockTradingApplication.DTOs;
using StockTradingApplication.Entities;

namespace StockTradingApplication.Services;

public interface IStockDbService
{
    Task<List<Stock>> GetStocksAsync();
    Task<Stock?> GetStockAsync(string symbol);
    Task HandleExternalProviderData(List<StockDataDto> stockDataDtos);
}