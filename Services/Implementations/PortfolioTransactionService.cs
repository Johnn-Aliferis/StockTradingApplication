using System.Net;
using AutoMapper;
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
    IPortfolioTransactionRepository portfolioTransactionRepository, 
    IStockDbService stockDbService,
    IMapper mapper)
    : IPortfolioTransactionService
{
    public async Task<PortfolioTransactionResponseDto> BuyStockAsync(PortfolioTransactionRequestDto portfolioTransactionRequestDto,
        long portfolioId)
    {
        ValidationService.ValidateTransactionRequestInput(portfolioTransactionRequestDto);
        var transaction = await HandleBuyStock(portfolioTransactionRequestDto, portfolioId);
        return mapper.Map<PortfolioTransactionResponseDto>(transaction);
    }

    public async Task<PortfolioTransactionResponseDto> SellStockAsync(
        PortfolioTransactionRequestDto portfolioTransactionRequestDto,
        long portfolioId)
    {
        ValidationService.ValidateTransactionRequestInput(portfolioTransactionRequestDto);
        var transaction = await HandleSellStock(portfolioTransactionRequestDto, portfolioId);
        return mapper.Map<PortfolioTransactionResponseDto>(transaction);
    }

    private async Task<PortfolioTransaction> HandleBuyStock(
        PortfolioTransactionRequestDto portfolioTransactionRequestDto, long portfolioId)
    {
        var portfolio = await GetPortfolioByGivenId(portfolioId);
        var stockData = await GetStockDataByGivenSymbolAsync(portfolioTransactionRequestDto.Symbol);
        
        var requestedPrice = portfolioTransactionRequestDto.Quantity * stockData.Close;

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
                StockPriceAtTransaction = stockData.Close,
                PortfolioId = portfolioId,
                TransactionType = TransactionTypeEnum.Buy.ToString(),
                Quantity = portfolioTransactionRequestDto.Quantity,
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

            return transaction;
        }
        catch (DbUpdateConcurrencyException)
        {
            await unitOfWork.RollbackAsync();
            throw new PortfolioTransactionException("Portfolio has already been updated by another entity",
                HttpStatusCode.Conflict);
        }
    }

    private async Task<PortfolioTransaction> HandleSellStock(
        PortfolioTransactionRequestDto portfolioTransactionRequestDto, long portfolioId)
    {
        var portfolio = await GetPortfolioByGivenId(portfolioId);
        var stockData = await GetStockDataByGivenSymbolAsync(portfolioTransactionRequestDto.Symbol);
        var portfolioHolding = await GetPortfolioHoldingByGivenId(portfolioId, stockData.Id);

        // Validation for sufficient shares 
        if (portfolioHolding.Quantity < portfolioTransactionRequestDto.Quantity)
        {
            throw new PortfolioTransactionException("Insufficient shares in portfolio to complete transaction",
                HttpStatusCode.BadRequest);
        }

        await unitOfWork.BeginTransactionAsync();

        try
        {
            portfolioHolding.Quantity -= portfolioTransactionRequestDto.Quantity;
            portfolio.CashBalance += portfolioTransactionRequestDto.Quantity * stockData.Close;

            // Creating corresponding transaction
            var transaction = new PortfolioTransaction
            {
                StockPriceAtTransaction = stockData.Close,
                PortfolioId = portfolioId,
                TransactionType = TransactionTypeEnum.Sell.ToString(),
                Quantity = portfolioTransactionRequestDto.Quantity,
                StockId = stockData.Id,
            };
            await portfolioTransactionRepository.CreateTransaction(transaction);

            // Since both entities are attached , simply commit the transaction so EF will persist changes.
            await unitOfWork.CommitAsync();

            return transaction;
        }
        catch (DbUpdateConcurrencyException)
        {
            await unitOfWork.RollbackAsync();
            throw new PortfolioTransactionException("Portfolio has already been updated by another entity",
                HttpStatusCode.Conflict);
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

    private async Task<Portfolio> GetPortfolioByGivenId(long portfolioId)
    {
        var portfolio = await portfolioRepository.FindPortfolioById(portfolioId);

        if (portfolio is null)
        {
            throw new PortfolioException($"Portfolio with ID {portfolioId} not found", HttpStatusCode.NotFound);
        }

        return portfolio;
    }

    private async Task<PortfolioHolding> GetPortfolioHoldingByGivenId(long portfolioId, long stockId)
    {
        var portfolioHolding =
            await portfolioRepository.FindPortfolioHoldingByPortfolioId(portfolioId, stockId);

        if (portfolioHolding is null)
        {
            throw new PortfolioException(
                $"Portfolio with ID {portfolioId} doesn't contain shares of the requested stock",
                HttpStatusCode.NotFound);
        }

        return portfolioHolding;
    }
}