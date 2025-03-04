using StockTradingApplication.Entities;

namespace StockTradingApplication.Repository;

public interface IStockReaderRepository
{
    Task<List<Stock>> GetStocksAsync();
    Task<List<string>> GetSymbolsAsync();
    Task<Stock?> GetStockAsync(string symbol); 
}