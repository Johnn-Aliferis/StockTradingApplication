using Microsoft.AspNetCore.Mvc;
using StockTradingApplication.DTOs;

namespace StockTradingApplication.Services.Interfaces;

public interface IPortfolioTransactionService
{
    Task BuyStockAsync(PortfolioTransactionRequestDto portfolioTransactionRequestDto, long portfolioId);

    Task SellStockAsync(PortfolioTransactionRequestDto portfolioTransactionRequestDto, long portfolioId);
}