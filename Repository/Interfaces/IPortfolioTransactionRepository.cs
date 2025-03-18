using StockTradingApplication.Entities;

namespace StockTradingApplication.Repository.Interfaces;

public interface IPortfolioTransactionRepository
{
    Task<PortfolioTransaction?> CreateTransaction(PortfolioTransaction portfolioTransaction);
}