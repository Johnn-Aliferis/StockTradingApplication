using StockTradingApplication.DTOs;
using StockTradingApplication.Entities;

namespace StockTradingApplication.Services.Interfaces;

public interface IPortfolioService
{
    Task<PortfolioResponseDto> CreatePortfolioAsync(PortfolioRequestDto createPortfolioRequest);
    Task<PortfolioResponseDto> AddPortfolioBalance(PortfolioRequestDto createPortfolioRequest, long portfolioId);
    Task DeletePortfolioAsync(long portfolioId);
    Task<PortfolioResponseDto?> GetPortfolioAsync(long portfolioId);
    Task<PortfolioHoldingResponseDto?> GetPortfolioHoldingByPortfolioAndStockIdAsync(long portfolioId, long stockId);
}