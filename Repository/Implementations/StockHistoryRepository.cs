using Microsoft.EntityFrameworkCore;
using StockTradingApplication.Entities;
using StockTradingApplication.Persistence;
using StockTradingApplication.Repository.Interfaces;

namespace StockTradingApplication.Repository.Implementations;

public class StockHistoryRepository(AppDbContext context) : IStockHistoryRepository
{
    private readonly DbSet<StockHistory> _stockHistories = context.Set<StockHistory>();

    public async Task<List<StockHistory>> GetStockHistory(string symbol)
    {
       return await _stockHistories.Where(sh => sh.Stock.Symbol == symbol).ToListAsync();
    }
    // For simplicity reasons for this DEMO , we leave with fetching all history from DB.
    
    // Performance Optimization Note: 
    // We can introduce the use of MATERIALIZED VIEWS during the insertion of the histories and update them periodically 
    // to have precomputed data which will help with performance. 
    // Another solution would be to use partitioning of the datatable , with some filtering with start / end date to get 
    // only the data user needs , thus reducing costs.
}