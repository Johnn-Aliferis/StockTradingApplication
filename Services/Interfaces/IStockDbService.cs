using StockTradingApplication.DTOs;

namespace StockTradingApplication.Services.Interfaces;

public interface IStockDbService
{
    Task<List<StockDataDto>> GetStocksAsync();
    Task<StockDataDto?> GetStockAsync(string symbol);
    Task<List<StockDataDto>> HandleExternalProviderData(List<StockDataDto> stockDataDtos);
}