using Microsoft.EntityFrameworkCore;
using StockTradingApplication.Data;
using StockTradingApplication.Entities;

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

    public async Task SaveOrUpdateStockAsync(Stock stock)
    {
        var existingStock = _stocks.FirstOrDefault(s => s.Symbol == stock.Symbol);

        if (existingStock != null)
        {
            existingStock.Price = stock.Price;
            existingStock.Name = stock.Name;
            existingStock.Currency = stock.Currency;
            existingStock.UpdatedAt = DateTime.UtcNow;
            _stocks.Update(existingStock);
        }
        else
        {
            await _stocks.AddAsync(stock);
        }

        await context.SaveChangesAsync();
    }
}