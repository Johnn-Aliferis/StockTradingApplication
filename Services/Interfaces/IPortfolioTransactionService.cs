using StockTradingApplication.DTOs;

namespace StockTradingApplication.Services.Interfaces;

public interface IPortfolioTransactionService
{
    Task<PortfolioTransactionResponseDto> BuyStockAsync(PortfolioTransactionRequestDto portfolioTransactionRequestDto, long portfolioId);

    Task<PortfolioTransactionResponseDto> SellStockAsync(PortfolioTransactionRequestDto portfolioTransactionRequestDto, long portfolioId);
}