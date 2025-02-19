using System.Net;
using Microsoft.EntityFrameworkCore;
using StockTradingApplication.Entities;
using StockTradingApplication.Exceptions;
using StockTradingApplication.Persistence;

namespace StockTradingApplication.Repository;

public class StockRepository(AppDbContext context) : IStockRepository
{
    private readonly DbSet<Stock> _stocks = context.Set<Stock>();
    private readonly DbSet<StockHistory> _stockHistories = context.Set<StockHistory>();


    public async Task<List<Stock>> GetStocksAsync()
    {
        return await _stocks.ToListAsync();
    }

    public async Task<Stock?> GetStockAsync(string symbol)
    {
        return await _stocks.FirstOrDefaultAsync(s => s.Symbol == symbol);
    }

    public async Task HandleInsertAndUpdateBulkOperationAsync(List<Stock> stocksToInsert,
        List<Stock> stocksToUpdate, List<StockHistory> stocksToInsertHistory)
    {
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            _stocks.AddRange(stocksToInsert);
            _stocks.UpdateRange(stocksToUpdate);
            _stockHistories.AddRange(stocksToInsertHistory);
            
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            throw new GeneralException($"An error occured while inserting stocks: {e.Message}" ,HttpStatusCode.InternalServerError);
        }
    }
}