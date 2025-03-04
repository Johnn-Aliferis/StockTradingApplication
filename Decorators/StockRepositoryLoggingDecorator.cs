using StockTradingApplication.DTOs;
using StockTradingApplication.Entities;
using StockTradingApplication.Repository;

namespace StockTradingApplication.Decorators;

public class StockRepositoryLoggingDecorator(IStockRepository decoratedRepository, ILogger<StockRepositoryLoggingDecorator> logger) : IStockRepository
{

    public async Task<List<Stock>> GetStocksAsync()
    {
        try
        {
            logger.LogInformation("Fetching all stocks from database at {Timestamp}.", DateTime.UtcNow);
            var result = await decoratedRepository.GetStocksAsync();
            logger.LogInformation("Successfully fetched {Count} stocks at {Timestamp}.", result.Count, DateTime.UtcNow);
            return result;
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error while fetching stocks at {Timestamp}.", DateTime.UtcNow);
            throw;
        }
    }

    public async Task<Stock?> GetStockAsync(string symbol)
    {
        try
        {
            logger.LogInformation("Fetching stock by symbol from database at {Timestamp}.", DateTime.UtcNow);
            var result = await decoratedRepository.GetStockAsync(symbol);
            if (result == null)
            {
                logger.LogInformation("No stock found for symbol {Symbol}.", symbol);
            }

            logger.LogInformation("Successfully fetched stock at {Timestamp}" ,  DateTime.UtcNow);
            return result;
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error while fetching stock by symbol at {Timestamp}.", DateTime.UtcNow);
            throw;
        }
    }

    public async Task<List<Stock>> HandleInsertAndUpdateBulkOperationAsync(SqlQueryDto mergeSqlQueryDto, SqlQueryDto historySqlQueryDto)
    {
        try
        {
            logger.LogInformation("Starting bulk insert/update operation at {Timestamp}.", DateTime.UtcNow);
            var result = await decoratedRepository.HandleInsertAndUpdateBulkOperationAsync(mergeSqlQueryDto, historySqlQueryDto);
            logger.LogInformation("Successfully completed bulk insert/update operation at {Timestamp}.", DateTime.UtcNow);
            return result;
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error while inserting/updating stocks at {Timestamp}.", DateTime.UtcNow);
            throw;
        }
    }
}