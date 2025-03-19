using StockTradingApplication.Entities;

namespace StockTradingApplication.Repository.Interfaces;

public interface IPortfolioRepository
{
    Task<Portfolio?> GetPortfolioByUserIdAsync(long userId);
    Task<Portfolio?> FindPortfolioById(long portfolioId);
    Task DeletePortfolioAsync(Portfolio portfolioId);
    Task<Portfolio> SavePortfolioAsync(Portfolio portfolio);
    Task<PortfolioHolding> SavePortfolioHoldingAsync(PortfolioHolding portfolioHolding);
    Task<PortfolioHolding?> FindPortfolioHoldingByPortfolioId(long portfolioId, long stockId);
}