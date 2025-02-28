using System.Net;
using Microsoft.EntityFrameworkCore;
using StockTradingApplication.DTOs;
using StockTradingApplication.Entities;
using StockTradingApplication.Exceptions;
using StockTradingApplication.Persistence;

namespace StockTradingApplication.Repository;

public class StockRepository(AppDbContext context, ILogger<StockRepository> logger) : IStockRepository
{
    private readonly DbSet<Stock> _stocks = context.Set<Stock>();
    private readonly DbSet<StockHistory> _stockHistories = context.Set<StockHistory>();


    public async Task<List<Stock>> GetStocksAsync()
    {
        return await _stocks.ToListAsync();
    }

    public async Task<List<string>> GetSymbolsAsync()
    {
        return await _stocks.Select(s => s.Symbol).ToListAsync();
    }

    public async Task<Stock?> GetStockAsync(string symbol)
    {
        return await _stocks.FirstOrDefaultAsync(s => s.Symbol == symbol);
    }

    public async Task HandleInsertAndUpdateBulkOperationAsync(SqlQueryDto mergeSqlQueryDto, SqlQueryDto historySqlQueryDto)
    {
        await using var transaction = await context.Database.BeginTransactionAsync();
        
        try
        {
            await PersistQueryAsync(historySqlQueryDto);
            await PersistQueryAsync(mergeSqlQueryDto);
            
            await transaction.CommitAsync();
        }
        catch (Exception e)
        {
            // Logging to be added via decorator patter later .
            await transaction.RollbackAsync();
            throw new GeneralException($"An error occured while inserting stocks: {e.Message}" ,HttpStatusCode.InternalServerError);
        }
    }

    private async Task PersistQueryAsync(SqlQueryDto sqlQueryDto)
    {
        if (!string.IsNullOrEmpty(sqlQueryDto.Query))
        {
            await context.Database.ExecuteSqlRawAsync(sqlQueryDto.Query, sqlQueryDto.Parameters);
        }
    }
}