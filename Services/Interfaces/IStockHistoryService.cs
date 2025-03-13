using StockTradingApplication.DTOs;

namespace StockTradingApplication.Services.Interfaces;

public interface IStockHistoryService
{
    Task<List<StockHistoryDto>> GetStockHistoryAsync(string symbol);
}