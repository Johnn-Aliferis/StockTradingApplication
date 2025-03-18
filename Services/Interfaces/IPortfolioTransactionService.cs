using Microsoft.AspNetCore.Mvc;
using StockTradingApplication.DTOs;

namespace StockTradingApplication.Services.Interfaces;

public interface IPortfolioTransactionService
{
    Task<IActionResult> BuyStockAsync(PortfolioTransactionRequestDto portfolioTransactionRequestDto);
    
    Task<IActionResult> SellStockAsync(PortfolioTransactionRequestDto portfolioTransactionRequestDto);
}