using StockTradingApplication.DTOs;
using StockTradingApplication.Entities;

namespace StockTradingApplication.Repository;

public interface IStockRepository
{
    Task<List<Stock>> GetStocksAsync();
    Task<Stock?> GetStockAsync(string symbol);
    Task<List<Stock>> HandleInsertAndUpdateBulkOperationAsync(SqlQueryDto mergeSqlQueryDto, SqlQueryDto historySqlQueryDto);
}