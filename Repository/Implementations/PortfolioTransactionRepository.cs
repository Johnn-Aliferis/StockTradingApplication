using Microsoft.EntityFrameworkCore;
using StockTradingApplication.Entities;
using StockTradingApplication.Persistence;
using StockTradingApplication.Repository.Interfaces;

namespace StockTradingApplication.Repository.Implementations;

public class PortfolioTransactionRepository(AppDbContext context) : IPortfolioTransactionRepository
{
    private readonly DbSet<PortfolioTransaction> _transactions = context.Set<PortfolioTransaction>();

    public async Task<PortfolioTransaction?> CreateTransaction(PortfolioTransaction portfolioTransaction)
    {
        var createdTransaction = await _transactions.AddAsync(portfolioTransaction);
        await context.SaveChangesAsync();
        return createdTransaction.Entity;
    }
}