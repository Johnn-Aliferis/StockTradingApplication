using StockTradingApplication.DTOs;

namespace StockTradingApplication.Services.Interfaces;

public interface IExternalStockService
{
    Task<IEnumerable<StockDataDto>> GetStockData();
}