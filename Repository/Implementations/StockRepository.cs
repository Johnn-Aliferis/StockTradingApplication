using Microsoft.EntityFrameworkCore;
using StockTradingApplication.DTOs;
using StockTradingApplication.Entities;
using StockTradingApplication.Persistence;

namespace StockTradingApplication.Repository;

public class StockRepository(AppDbContext context) : IStockRepository
{
    private readonly DbSet<Stock> _stocks = context.Set<Stock>();

    public async Task<List<Stock>> GetStocksAsync()
    {
        return await _stocks.ToListAsync();
    }

    public async Task<Stock?> GetStockAsync(string symbol)
    {
        return await _stocks.FirstOrDefaultAsync(s => s.Symbol == symbol);
    }

    public async Task<List<Stock>> HandleInsertAndUpdateBulkOperationAsync(SqlQueryDto mergeSqlQueryDto, SqlQueryDto historySqlQueryDto)
    {
        await using var transaction = await context.Database.BeginTransactionAsync();
        
        try
        {
            await PersistQueryAsync(historySqlQueryDto);
            var result = await PersistQueryAsyncAndReturn(mergeSqlQueryDto);
            
            await transaction.CommitAsync();
            return result;
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    private async Task PersistQueryAsync(SqlQueryDto sqlQueryDto)
    {
        if (!string.IsNullOrEmpty(sqlQueryDto.Query))
        {
            await context.Database.ExecuteSqlRawAsync(sqlQueryDto.Query, sqlQueryDto.Parameters);
        }
    }

    private async Task<List<Stock>> PersistQueryAsyncAndReturn(SqlQueryDto sqlQueryDto)
    {
        if (string.IsNullOrEmpty(sqlQueryDto.Query))
        {
            return [];
        }
        return await _stocks.FromSqlRaw(sqlQueryDto.Query, sqlQueryDto.Parameters).ToListAsync();
    }
}