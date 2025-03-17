using System.Net;
using Microsoft.EntityFrameworkCore;
using StockTradingApplication.Entities;
using StockTradingApplication.Exceptions;
using StockTradingApplication.Persistence;
using StockTradingApplication.Repository.Interfaces;

namespace StockTradingApplication.Repository.Implementations;

public class PortfolioRepository(AppDbContext context, ILogger<PortfolioRepository> logger) : IPortfolioRepository
{
    private readonly DbSet<Portfolio> _portfolios = context.Set<Portfolio>();
    private readonly DbSet<PortfolioBalance> _portfoliosBalances = context.Set<PortfolioBalance>();

    public async Task<Portfolio?> GetPortfolioAsync(long userId)
    {
        return await _portfolios.FirstOrDefaultAsync(portfolio => portfolio.UserId == userId);
    }

    public async Task<Portfolio?> FindPortfolioById(long portfolioId)
    {
        return await _portfolios.FindAsync(portfolioId);
    }

    public async Task DeletePortfolioAsync(Portfolio portfolio)
    {
        _portfolios.Remove(portfolio);
        await context.SaveChangesAsync();
    }

    public async Task<Portfolio> SavePortfolioAsync(Portfolio portfolio)
    {
        // Utilizing transaction to ensure atomicity
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            await _portfolios.AddAsync(portfolio);
            await context.SaveChangesAsync();

            var portfolioBalance = new PortfolioBalance
            {
                PortfolioId = portfolio.Id,
                Balance = portfolio.CashBalance,
            };

            await _portfoliosBalances.AddAsync(portfolioBalance);
            await context.SaveChangesAsync();
            await transaction.CommitAsync();

            return portfolio;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            logger.LogError("An error occurred with message : {}", ex.ToString());
            throw new PortfolioException("An unexpected error occurred during portfolio creation. ", HttpStatusCode.InternalServerError);
        }
    }
}