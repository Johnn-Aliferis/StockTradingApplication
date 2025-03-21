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
    private readonly DbSet<PortfolioHolding> _portfoliosHoldings = context.Set<PortfolioHolding>();

    public async Task<Portfolio?> GetPortfolioByUserIdAsync(long userId)
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
        try
        {
            await _portfolios.AddAsync(portfolio);
            await context.SaveChangesAsync();
            return portfolio;
        }
        catch (Exception ex)
        {
            logger.LogError("An error occurred with message : {}", ex.ToString());
            throw new PortfolioException("An unexpected error occurred during portfolio creation. ",
                HttpStatusCode.InternalServerError);
        }
    }

    public async Task<PortfolioHolding> SavePortfolioHoldingAsync(PortfolioHolding portfolioHolding)
    {
        var existingHolding = await _portfoliosHoldings.FindAsync(portfolioHolding.Id);
        if (existingHolding != null)
        {
            context.Entry(existingHolding).CurrentValues.SetValues(portfolioHolding);
        }
        else
        {
            await _portfoliosHoldings.AddAsync(portfolioHolding);
        }

        await context.SaveChangesAsync();
        return portfolioHolding;
    }

    public async Task<PortfolioHolding?> FindPortfolioHoldingByPortfolioId(long portfolioId, long stockId)
    {
        return await _portfoliosHoldings.FirstOrDefaultAsync(holding =>
            holding.PortfolioId == portfolioId && holding.StockId == stockId);
    }
}