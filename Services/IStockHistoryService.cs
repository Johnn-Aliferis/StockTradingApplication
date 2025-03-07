using StockTradingApplication.DTOs;

namespace StockTradingApplication.Services;

public interface IStockHistoryService
{
    Task<List<StockHistoryDto>> GetStockHistoryAsync(string symbol);
}