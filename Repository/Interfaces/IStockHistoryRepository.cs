using StockTradingApplication.Entities;

namespace StockTradingApplication.Repository;

public interface IStockHistoryRepository

{
    Task<List<StockHistory>> GetStockHistory(string symbol);
}