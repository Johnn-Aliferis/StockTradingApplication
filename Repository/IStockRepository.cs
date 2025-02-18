using StockTradingApplication.Entities;

namespace StockTradingApplication.Repository;

public interface IStockRepository
{
    Task<List<Stock>> GetStocksAsync();
    Task<Stock?> GetStockAsync(string symbol); 
    Task SaveOrUpdateStockAsync(Stock stock);
}