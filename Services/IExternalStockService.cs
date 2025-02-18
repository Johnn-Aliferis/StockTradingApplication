using StockTradingApplication.DTOs;

namespace StockTradingApplication.Services;

public interface IExternalStockService
{
    Task<IEnumerable<StockDataDto>> GetStockData();
}