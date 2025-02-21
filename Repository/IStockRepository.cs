using StockTradingApplication.DTOs;
using StockTradingApplication.Entities;

namespace StockTradingApplication.Repository;

public interface IStockRepository
{
    Task<List<Stock>> GetStocksAsync();
    Task<List<string>> GetSymbolsAsync();
    Task<Stock?> GetStockAsync(string symbol); 
    Task HandleInsertAndUpdateBulkOperationAsync(SqlQueryDto mergeSqlQueryDto, SqlQueryDto historySqlQueryDto);
}