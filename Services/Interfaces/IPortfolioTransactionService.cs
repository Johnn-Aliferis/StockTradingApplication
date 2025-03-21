using Microsoft.AspNetCore.Mvc;
using StockTradingApplication.DTOs;
using StockTradingApplication.Entities;

namespace StockTradingApplication.Services.Interfaces;

public interface IPortfolioTransactionService
{
    Task<PortfolioTransaction> BuyStockAsync(PortfolioTransactionRequestDto portfolioTransactionRequestDto, long portfolioId);

    Task<PortfolioTransaction> SellStockAsync(PortfolioTransactionRequestDto portfolioTransactionRequestDto, long portfolioId);
}