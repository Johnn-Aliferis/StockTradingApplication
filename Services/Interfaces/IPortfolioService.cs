using StockTradingApplication.DTOs;

namespace StockTradingApplication.Services.Interfaces;

public interface IPortfolioService
{
    Task<PortfolioResponseDto> CreatePortfolioAsync(CreatePortfolioRequestDto createPortfolioRequest);

    Task DeletePortfolioAsync(long portfolioId);
}