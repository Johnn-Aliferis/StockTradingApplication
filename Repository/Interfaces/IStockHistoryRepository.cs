using StockTradingApplication.Entities;

namespace StockTradingApplication.Repository.Interfaces;

public interface IStockHistoryRepository

{
    Task<List<StockHistory>> GetStockHistory(string symbol);
}