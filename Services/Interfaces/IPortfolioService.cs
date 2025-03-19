using StockTradingApplication.DTOs;
using StockTradingApplication.Entities;

namespace StockTradingApplication.Services.Interfaces;

public interface IPortfolioService
{
    Task<PortfolioResponseDto> CreatePortfolioAsync(CreatePortfolioRequestDto createPortfolioRequest);
    Task DeletePortfolioAsync(long portfolioId);
    Task<PortfolioResponseDto?> GetPortfolioAsync(long portfolioId);
    Task<PortfolioHoldingResponseDto?> GetPortfolioHoldingByPortfolioAndStockIdAsync(long portfolioId, long stockId);
}