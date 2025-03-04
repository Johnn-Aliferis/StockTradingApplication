using StockTradingApplication.DTOs;

namespace StockTradingApplication.Repository;

public interface IStockWriterRepository
{
    Task HandleInsertAndUpdateBulkOperationAsync(SqlQueryDto mergeSqlQueryDto, SqlQueryDto historySqlQueryDto);
}