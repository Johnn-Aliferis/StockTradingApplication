using StockTradingApplication.Entities;
using StockTradingApplication.Repository.Interfaces;

namespace StockTradingApplication.Repository.Implementations;

public class PortfolioTransactionRepository : IPortfolioTransactionRepository
{
    public Task<PortfolioTransaction?> CreateTransaction(PortfolioTransaction portfolioTransaction)
    {
        throw new NotImplementedException();
    }
}