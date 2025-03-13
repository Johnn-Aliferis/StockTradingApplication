using StockTradingApplication.DTOs;
using StockTradingApplication.Entities;

namespace StockTradingApplication.Services.Interfaces;

public interface IPortfolioService
{
    Task<Portfolio> CreatePortfolioAsync(CreatePortfolioRequestDto createPortfolioRequest);

    Task DeletePortfolioAsync(long portfolioId);
}