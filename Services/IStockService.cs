using StockTradingApplication.DTOs;

namespace StockTradingApplication.Services;

public interface IStockService
{
    Task<IEnumerable<StockData>> GetStockData();
}