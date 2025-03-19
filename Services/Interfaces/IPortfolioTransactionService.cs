using Microsoft.AspNetCore.Mvc;
using StockTradingApplication.DTOs;

namespace StockTradingApplication.Services.Interfaces;

public interface IPortfolioTransactionService
{
    Task<IActionResult> BuyStockAsync(PortfolioTransactionRequestDto portfolioTransactionRequestDto, long portfolioId);
    
    Task<IActionResult> SellStockAsync(PortfolioTransactionRequestDto portfolioTransactionRequestDto, long portfolioId);
}