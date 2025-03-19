using System.Net;
using Microsoft.AspNetCore.Mvc;
using StockTradingApplication.DTOs;
using StockTradingApplication.Exceptions;
using StockTradingApplication.Repository.Interfaces;
using StockTradingApplication.Services.Interfaces;

namespace StockTradingApplication.Services.Implementations;

public class PortfolioTransactionService(
    IUnitOfWork unitOfWork,
    IPortfolioService portfolioService,
    IStockDbService stockDbService, 
    ILogger<PortfolioTransactionService> logger)
    : IPortfolioTransactionService
{
    public async Task<IActionResult> BuyStockAsync(PortfolioTransactionRequestDto portfolioTransactionRequestDto,
        long portfolioId)
    {
        ValidationService.ValidateTransactionRequestInput(portfolioTransactionRequestDto);
        await ValidateBuyStock(portfolioTransactionRequestDto, portfolioId);
        throw new NotImplementedException();
    }

    public async Task<IActionResult> SellStockAsync(PortfolioTransactionRequestDto portfolioTransactionRequestDto,
        long portfolioId)
    {
        ValidationService.ValidateTransactionRequestInput(portfolioTransactionRequestDto);
        await ValidateSellStock(portfolioTransactionRequestDto, portfolioId);
        throw new NotImplementedException();
    }

    
    private async Task ValidateSellStock(PortfolioTransactionRequestDto portfolioTransactionRequestDto, long portfolioId)
    {
        var stockData = await GetStockDataByGivenSymbolAsync(portfolioTransactionRequestDto.Symbol);
        var portfolioHoldingData = await GetPortfolioHoldingByGivenId(portfolioId, stockData.Id);
        ValidateQuantity(portfolioTransactionRequestDto, portfolioHoldingData);
    }

    private async Task ValidateBuyStock(PortfolioTransactionRequestDto portfolioTransactionRequestDto, long portfolioId)
    {
        var portfolioData = await GetPortfolioByGivenId(portfolioId);
        var stockData = await GetStockDataByGivenSymbolAsync(portfolioTransactionRequestDto.Symbol);
        ValidateFunds(portfolioTransactionRequestDto, portfolioData, stockData);
    }

    private static void ValidateFunds(PortfolioTransactionRequestDto portfolioTransactionRequestDto,
        PortfolioResponseDto portfolioData, StockDataDto stockData)
    {
        var requestedPrice = portfolioTransactionRequestDto.Quantity * stockData.Close;

        if (portfolioData.CashBalance < requestedPrice)
        {
            throw new PortfolioTransactionException("Insufficient funds to complete transaction",
                HttpStatusCode.BadRequest);
        }
    }
    
    private static void ValidateQuantity(PortfolioTransactionRequestDto portfolioTransactionRequestDto,
        PortfolioHoldingResponseDto portfolioHoldingData)
    {
        if (portfolioHoldingData.StockQuantity < portfolioTransactionRequestDto.Quantity)
        {
            throw new PortfolioTransactionException("Insufficient stock holdings to complete transaction",
                HttpStatusCode.BadRequest);
        }
    }

    private async Task<StockDataDto> GetStockDataByGivenSymbolAsync(string symbol)
    {
        var stock = await stockDbService.GetStockAsync(symbol);

        if (stock is null)
        {
            throw new PortfolioTransactionException("Could not find given stock", HttpStatusCode.NotFound);
        }

        return stock;
    }

    private async Task<PortfolioResponseDto> GetPortfolioByGivenId(long portfolioId)
    {
        var portfolio = await portfolioService.GetPortfolioAsync(portfolioId);

        if (portfolio is null)
        {
            throw new PortfolioException($"Portfolio with ID {portfolioId} not found", HttpStatusCode.NotFound);
        }

        return portfolio;
    }
    
    private async Task<PortfolioHoldingResponseDto> GetPortfolioHoldingByGivenId(long portfolioId, long stockId)
    {
        var portfolioHolding = await portfolioService.GetPortfolioHoldingAsync(portfolioId, stockId);

        if (portfolioHolding is null)
        {
            throw new PortfolioException($"Portfolio Holding with for portfolio with ID {portfolioId} not found", HttpStatusCode.NotFound);
        }

        return portfolioHolding;
    }


    // TODO : Algorithm for locking for selling/buying :
    //      1) Utility Validation --> Request is correct -- Done 
    //      2) Business Validation --> Portfolio existence , balance in portfolio is sufficient depending on action etc -- Done , needs testing!!
    //      3) Begin Transaction 
    //      4) Modify and call all repositories needed
    //      5) Attempt to Save Changes --> RowVersion check for optimistic locking
    //      6) If error , rollback and send message appropriate to user
    //      7) If all ok , Save changes in DB and notify User.
}