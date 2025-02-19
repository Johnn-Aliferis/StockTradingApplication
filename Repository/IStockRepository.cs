using StockTradingApplication.DTOs;
using StockTradingApplication.Entities;

namespace StockTradingApplication.Repository;

public interface IStockRepository
{
    Task<List<Stock>> GetStocksAsync();
    Task<Stock?> GetStockAsync(string symbol); 
    Task HandleInsertAndUpdateBulkOperationAsync(List<Stock> stocksToInsert,
        List<Stock> stocksToUpdate, List<StockHistory> stocksToInsertHistory);
}