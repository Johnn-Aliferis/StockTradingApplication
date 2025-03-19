using System.Net;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StockTradingApplication.Common;
using StockTradingApplication.DTOs;
using StockTradingApplication.Entities;
using StockTradingApplication.Exceptions;
using StockTradingApplication.Repository.Interfaces;
using StockTradingApplication.Services.Interfaces;

namespace StockTradingApplication.Services.Implementations;

public class PortfolioTransactionService(
    IUnitOfWork unitOfWork,
    IPortfolioRepository portfolioRepository,
    IStockRepository stockRepository,
    IPortfolioTransactionRepository portfolioTransactionRepository)
    : IPortfolioTransactionService
{
    public async Task BuyStockAsync(PortfolioTransactionRequestDto portfolioTransactionRequestDto,
        long portfolioId)
    {
        ValidationService.ValidateTransactionRequestInput(portfolioTransactionRequestDto);
        await HandleBuyStock(portfolioTransactionRequestDto, portfolioId);
    }

    public async Task SellStockAsync(PortfolioTransactionRequestDto portfolioTransactionRequestDto,
        long portfolioId)
    {
        ValidationService.ValidateTransactionRequestInput(portfolioTransactionRequestDto);
        var portfolioData = await GetPortfolioByGivenId(portfolioId);

        throw new NotImplementedException();
    }

    private async Task HandleBuyStock(PortfolioTransactionRequestDto portfolioTransactionRequestDto, long portfolioId)
    {
        var portfolio = await GetPortfolioByGivenId(portfolioId);
        var stockData = await GetStockDataByGivenSymbolAsync(portfolioTransactionRequestDto.Symbol);

        var requestedPrice = portfolioTransactionRequestDto.Quantity * stockData.Price;

        // Validation for sufficient funds
        if (portfolio.CashBalance < requestedPrice)
        {
            throw new PortfolioTransactionException("Insufficient funds to complete transaction",
                HttpStatusCode.BadRequest);
        }

        await unitOfWork.BeginTransactionAsync();
        try
        {
            portfolio.CashBalance -= requestedPrice;

            // Creating corresponding transaction
            var transaction = new PortfolioTransaction
            {
                StockPriceAtTransaction = stockData.Price,
                PortfolioId = portfolioId,
                TransactionType = TransactionTypeEnum.Buy.ToString(),
                StockId = stockData.Id,
            };
            await portfolioTransactionRepository.CreateTransaction(transaction);

            // Finding holding if exists
            var portfolioHolding =
                await portfolioRepository.FindPortfolioHoldingByPortfolioId(portfolioId, stockData.Id);

            if (portfolioHolding is null)
            {
                portfolioHolding = new PortfolioHolding
                {
                    Quantity = portfolioTransactionRequestDto.Quantity,
                    PortfolioId = portfolioId,
                    StockId = stockData.Id
                };
                // Explicitly add the new holding to the repository so it's tracked
                await portfolioRepository.SavePortfolioHoldingAsync(portfolioHolding);
            }
            else
            {
                portfolioHolding.Quantity += portfolioTransactionRequestDto.Quantity;
            }

            await unitOfWork.CommitAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            await unitOfWork.RollbackAsync();
            throw new PortfolioTransactionException("Portfolio has already been updated by another entity",
                HttpStatusCode.Conflict);
        }
    }

    private async Task HandleSellStock()
    {
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

    private async Task<Stock> GetStockDataByGivenSymbolAsync(string symbol)
    {
        var stock = await stockRepository.GetStockAsync(symbol);

        if (stock is null)
        {
            throw new PortfolioTransactionException("Could not find given stock", HttpStatusCode.NotFound);
        }

        return stock;
    }

    private async Task<Portfolio> GetPortfolioByGivenId(long portfolioId)
    {
        var portfolio = await portfolioRepository.FindPortfolioById(portfolioId);

        if (portfolio is null)
        {
            throw new PortfolioException($"Portfolio with ID {portfolioId} not found", HttpStatusCode.NotFound);
        }

        return portfolio;
    }


    // TODO : Algorithm for locking for selling/buying :
    //      1) Utility Validation --> Request is correct -- Done 
    //      2) Business Validation --> Portfolio existence , balance in portfolio is sufficient depending on action etc -- Done , needs testing!!
    //      3) Begin Transaction -- > Done for buying
    //      4) Modify and call all repositories needed -- done for buying
    //      5) Attempt to Save Changes --> RowVersion check for optimistic locking -- done for buying
    //      6) If error , rollback and send message appropriate to user -- exception is thrown so we are good. 
    //      7) If all ok , Save changes in DB and notify User.
    
    // Todo : Implementation done for buy stock. Needs in every step manual testing to see if all is good. Also
    //      Also to manually test again entire flow , because we have removed an entity !!! 
}