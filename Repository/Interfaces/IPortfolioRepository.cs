using StockTradingApplication.Entities;

namespace StockTradingApplication.Repository.Interfaces;

public interface IPortfolioRepository
{
    Task<Portfolio?> GetPortfolioAsync(long userId);
    Task<Portfolio?> FindPortfolioById(long portfolioId);
    Task DeletePortfolioAsync(Portfolio portfolioId);
    
    Task<Portfolio> SavePortfolioAsync(Portfolio portfolio);
}