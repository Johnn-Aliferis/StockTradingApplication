using Microsoft.EntityFrameworkCore;
using StockTradingApplication.Entities;
using StockTradingApplication.Persistence;

namespace StockTradingApplication.Repository;

public class StockHistoryRepository(AppDbContext context) : IStockHistoryRepository
{
    private readonly DbSet<StockHistory> _stockHistories = context.Set<StockHistory>();

    public async Task<List<StockHistory>> GetStockHistory(string symbol)
    {
       return await _stockHistories.Where(sh => sh.Stock.Symbol == symbol).ToListAsync();
    }
    // For performance reasons , we can include pagination , and filtering based on start Date and end Date.
    // For simplicity reasons for this DEMO , we leave with fetching all history from DB.
}