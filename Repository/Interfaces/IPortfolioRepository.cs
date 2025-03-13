using StockTradingApplication.Entities;

namespace StockTradingApplication.Repository.Interfaces;

public interface IPortfolioRepository
{
    Task<Portfolio?> GetPortfolioAsync(long userId);
    Task DeletePortfolioAsync(long portfolioId);
    
    Task<Portfolio> SavePortfolioAsync(Portfolio portfolio);
}