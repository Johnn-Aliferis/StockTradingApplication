using StockTradingApplication.DTOs;

namespace StockTradingApplication.Services.Interfaces;

public interface IPortfolioService
{
    Task<PortfolioResponseDto> CreatePortfolioAsync(CreatePortfolioRequestDto createPortfolioRequest);
    Task DeletePortfolioAsync(long portfolioId);
    Task<PortfolioResponseDto?> GetPortfolioAsync(long portfolioId);
    Task<PortfolioBalanceResponseDto?> GetPortfolioBalanceAsync(long portfolioId);
    Task<PortfolioHoldingResponseDto?> GetPortfolioHoldingAsync(long portfolioId, long stockId);
}