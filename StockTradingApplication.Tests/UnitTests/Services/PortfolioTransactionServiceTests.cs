using System.Net;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Moq;
using StockTradingApplication.Common;
using StockTradingApplication.DTOs;
using StockTradingApplication.Entities;
using StockTradingApplication.Exceptions;
using StockTradingApplication.Repository.Interfaces;
using StockTradingApplication.Services.Implementations;
using StockTradingApplication.Services.Interfaces;

namespace StockTradingApplication.Tests.UnitTests.Services;

public class PortfolioTransactionServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IPortfolioRepository> _portfolioRepositoryMock;
    private readonly Mock<IPortfolioTransactionRepository> _portfolioTransactionRepositoryMock;
    private readonly Mock<IStockDbService> _stockDbServiceMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly PortfolioTransactionService _service;

    public PortfolioTransactionServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _portfolioRepositoryMock = new Mock<IPortfolioRepository>();
        _portfolioTransactionRepositoryMock = new Mock<IPortfolioTransactionRepository>();
        _stockDbServiceMock = new Mock<IStockDbService>();
        _mapperMock = new Mock<IMapper>();

        _service = new PortfolioTransactionService(
            _unitOfWorkMock.Object,
            _portfolioRepositoryMock.Object,
            _portfolioTransactionRepositoryMock.Object,
            _stockDbServiceMock.Object,
            _mapperMock.Object);
    }

    [Fact]
    public async Task BuyStockAsync_Insufficient_Funds_Test()
    {
        const long portfolioId = 1L;

        var transactionRequest = new PortfolioTransactionRequestDto { Symbol = "AAPL", Quantity = 10 };

        var stockData = new StockDataDto { Symbol = "AAPL", Name = "Test AAPL", Close = 150m, Currency = "USD" };

        var portfolio = new Portfolio { Id = portfolioId, CashBalance = 500m };

        _portfolioRepositoryMock.Setup(r => r.FindPortfolioById(portfolioId)).ReturnsAsync(portfolio);
        _stockDbServiceMock.Setup(s => s.GetStockAsync("AAPL")).ReturnsAsync(stockData);

        var exception = await Assert.ThrowsAsync<PortfolioTransactionException>(
            () => _service.BuyStockAsync(transactionRequest, portfolioId));

        Assert.Equal("Insufficient funds to complete transaction", exception.Message);
        Assert.Equal(HttpStatusCode.BadRequest, exception.Status);
    }
    
    [Fact]
    public async Task BuyStockAsync_Insufficient_Shares_Test()
    {
        const long portfolioId = 1L;

        var transactionRequest = new PortfolioTransactionRequestDto { Symbol = "AAPL", Quantity = 10 };

        var stockData = new StockDataDto { Symbol = "AAPL", Name = "Test AAPL", Close = 150m, Currency = "USD" };
        
        var existingHolding = new PortfolioHolding { PortfolioId = portfolioId, StockId = 1, Quantity = 2 };

        var portfolio = new Portfolio { Id = portfolioId, CashBalance = 500m };

        _portfolioRepositoryMock.Setup(r => r.FindPortfolioById(portfolioId)).ReturnsAsync(portfolio);
        _stockDbServiceMock.Setup(s => s.GetStockAsync("AAPL")).ReturnsAsync(stockData);
        _portfolioRepositoryMock.Setup(r => r.FindPortfolioHoldingByPortfolioId(portfolioId, stockData.Id)).ReturnsAsync(existingHolding);

        var exception = await Assert.ThrowsAsync<PortfolioTransactionException>(
            () => _service.SellStockAsync(transactionRequest, portfolioId));

        Assert.Equal("Insufficient shares in portfolio to complete transaction", exception.Message);
        Assert.Equal(HttpStatusCode.BadRequest, exception.Status);
    }

    [Fact]
    public async Task BuyStockAsync_Stock_Symbol_NotFound_Test()
    {
        const long portfolioId = 1L;

        var transactionRequest = new PortfolioTransactionRequestDto { Symbol = "TEST", Quantity = 10 };
        StockDataDto? stockData = null;

        var portfolio = new Portfolio { Id = portfolioId, CashBalance = 500m };

        _portfolioRepositoryMock.Setup(r => r.FindPortfolioById(portfolioId)).ReturnsAsync(portfolio);
        _stockDbServiceMock.Setup(s => s.GetStockAsync("TEST")).ReturnsAsync(stockData);

        var exception = await Assert.ThrowsAsync<PortfolioTransactionException>(
            () => _service.BuyStockAsync(transactionRequest, portfolioId));

        Assert.Equal("Could not find given stock", exception.Message);
        Assert.Equal(HttpStatusCode.NotFound, exception.Status);
    }

    [Fact]
    public async Task BuyStockAsync_Portfolio_NotFound_Test()
    {
        const long portfolioId = 1L;

        var transactionRequest = new PortfolioTransactionRequestDto { Symbol = "TEST", Quantity = 10 };

        Portfolio? portfolio = null;

        _portfolioRepositoryMock.Setup(r => r.FindPortfolioById(portfolioId)).ReturnsAsync(portfolio);

        var exception = await Assert.ThrowsAsync<PortfolioException>(
            () => _service.BuyStockAsync(transactionRequest, portfolioId));

        Assert.Equal($"Portfolio with ID {portfolioId} not found", exception.Message);
        Assert.Equal(HttpStatusCode.NotFound, exception.Status);
    }

    [Fact]
    public async Task BuyStockAsync_Success_Test()
    {
        const long portfolioId = 1L;
        var transactionRequest = new PortfolioTransactionRequestDto { Symbol = "AAPL", Quantity = 5 };
        var stockData = new StockDataDto { Symbol = "AAPL", Name = "Test AAPL", Close = 150m, Currency = "USD" };
        var portfolio = new Portfolio { Id = portfolioId, CashBalance = 1000m };
        var existingHolding = new PortfolioHolding { PortfolioId = portfolioId, StockId = 1, Quantity = 2 };

        var transaction = new PortfolioTransaction
        {
            StockPriceAtTransaction = stockData.Close,
            PortfolioId = portfolioId,
            TransactionType = TransactionTypeEnum.Buy.ToString(),
            Quantity = transactionRequest.Quantity,
            StockId = stockData.Id
        };

        var portfolioTransactionResponseDto = new PortfolioTransactionResponseDto
        {
            Id = 1L,
            Quantity = 5,
            PortfolioId = 1L,
            TransactionType = TransactionTypeEnum.Buy.ToString(),
            StockPriceAtTransaction = 150m,
            StockId = 1
        };

        // Mock
        _portfolioRepositoryMock.Setup(r => r.FindPortfolioById(portfolioId)).ReturnsAsync(portfolio);
        _stockDbServiceMock.Setup(s => s.GetStockAsync("AAPL")).ReturnsAsync(stockData);
        _portfolioRepositoryMock.Setup(r => r.FindPortfolioHoldingByPortfolioId(portfolioId, stockData.Id))
            .ReturnsAsync(existingHolding);
        _unitOfWorkMock.Setup(u => u.BeginTransactionAsync()).Returns(Task.CompletedTask);
        _portfolioTransactionRepositoryMock.Setup(t => t.CreateTransaction(transaction))
            .ReturnsAsync((PortfolioTransaction?)null);
        _portfolioRepositoryMock.Setup(r => r.SavePortfolioHoldingAsync(It.IsAny<PortfolioHolding>()))
            .ReturnsAsync(existingHolding);
        _unitOfWorkMock.Setup(u => u.CommitAsync()).Returns(Task.CompletedTask);
        _mapperMock.Setup(m => m.Map<PortfolioTransactionResponseDto>(It.IsAny<PortfolioTransaction>()))
            .Returns(portfolioTransactionResponseDto);

        var result = await _service.BuyStockAsync(transactionRequest, portfolioId);

        Assert.NotNull(result);
        Assert.Equal(250m, portfolio.CashBalance);
        _portfolioTransactionRepositoryMock.Verify(t => t.CreateTransaction(It.IsAny<PortfolioTransaction>()),
            Times.Once);
        _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task SellStockAsync_Success_Test()
    {
        const long portfolioId = 1L;
        var transactionRequest = new PortfolioTransactionRequestDto { Symbol = "AAPL", Quantity = 4 };
        var stockData = new StockDataDto { Symbol = "AAPL", Name = "Test AAPL", Close = 150m, Currency = "USD" };
        var portfolio = new Portfolio { Id = portfolioId, CashBalance = 100m };
        var existingHolding = new PortfolioHolding { PortfolioId = portfolioId, StockId = 1, Quantity = 5 };
        var updatedHolding = new PortfolioHolding { PortfolioId = portfolioId, StockId = 1, Quantity = 1 };

        var transaction = new PortfolioTransaction
        {
            StockPriceAtTransaction = stockData.Close,
            PortfolioId = portfolioId,
            TransactionType = TransactionTypeEnum.Sell.ToString(),
            Quantity = transactionRequest.Quantity,
            StockId = stockData.Id
        };

        var portfolioTransactionResponseDto = new PortfolioTransactionResponseDto
        {
            Id = 1L,
            Quantity = 4,
            PortfolioId = 1L,
            TransactionType = TransactionTypeEnum.Buy.ToString(),
            StockPriceAtTransaction = 150m,
            StockId = 1
        };

        // Mock
        _portfolioRepositoryMock.Setup(r => r.FindPortfolioById(portfolioId)).ReturnsAsync(portfolio);
        _stockDbServiceMock.Setup(s => s.GetStockAsync("AAPL")).ReturnsAsync(stockData);
        _portfolioRepositoryMock.Setup(r => r.FindPortfolioHoldingByPortfolioId(portfolioId, stockData.Id))
            .ReturnsAsync(existingHolding);
        _unitOfWorkMock.Setup(u => u.BeginTransactionAsync()).Returns(Task.CompletedTask);
        _portfolioTransactionRepositoryMock.Setup(t => t.CreateTransaction(transaction))
            .ReturnsAsync((PortfolioTransaction?)null);
        _portfolioRepositoryMock.Setup(r => r.SavePortfolioHoldingAsync(It.IsAny<PortfolioHolding>()))
            .ReturnsAsync(updatedHolding);
        _unitOfWorkMock.Setup(u => u.CommitAsync()).Returns(Task.CompletedTask);
        _mapperMock.Setup(m => m.Map<PortfolioTransactionResponseDto>(It.IsAny<PortfolioTransaction>()))
            .Returns(portfolioTransactionResponseDto);

        var result = await _service.SellStockAsync(transactionRequest, portfolioId);

        Assert.NotNull(result);
        Assert.Equal(700m, portfolio.CashBalance); // Ensure balance was updated
        _portfolioTransactionRepositoryMock.Verify(t => t.CreateTransaction(It.IsAny<PortfolioTransaction>()),
            Times.Once);
        _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task BuyStockAsync_Concurrency_Test_Fail()
    {
        const long portfolioId = 1L;
        var transactionRequest = new PortfolioTransactionRequestDto { Symbol = "AAPL", Quantity = 4 };
        var stockData = new StockDataDto { Symbol = "AAPL", Name = "Test AAPL", Close = 150m, Currency = "USD" };
        var portfolio = new Portfolio { Id = portfolioId, CashBalance = 10000m };
        var existingHolding = new PortfolioHolding { PortfolioId = portfolioId, StockId = 1, Quantity = 2 };
        
        var transaction = new PortfolioTransaction
        {
            StockPriceAtTransaction = stockData.Close,
            PortfolioId = portfolioId,
            TransactionType = TransactionTypeEnum.Buy.ToString(),
            Quantity = transactionRequest.Quantity,
            StockId = stockData.Id
        };
        
        _portfolioRepositoryMock.Setup(r => r.FindPortfolioById(portfolioId)).ReturnsAsync(portfolio);

        _stockDbServiceMock.Setup(s => s.GetStockAsync("AAPL")).ReturnsAsync(stockData);

        _portfolioTransactionRepositoryMock.Setup(t => t.CreateTransaction(transaction)).ReturnsAsync((PortfolioTransaction?)null);

        _portfolioRepositoryMock.Setup(r => r.FindPortfolioHoldingByPortfolioId(portfolioId, stockData.Id)).ReturnsAsync(existingHolding);

        _portfolioRepositoryMock.Setup(r => r.SavePortfolioHoldingAsync(It.IsAny<PortfolioHolding>())).ReturnsAsync(existingHolding);

        _unitOfWorkMock.SetupSequence(u => u.CommitAsync()).Throws(new DbUpdateConcurrencyException()).Returns(Task.CompletedTask); 
        
        var task1 = Task.Run(() => _service.BuyStockAsync(transactionRequest, portfolioId));
        var task2 = Task.Run(() => _service.BuyStockAsync(transactionRequest, portfolioId));

        var exception =
            await Assert.ThrowsAsync<PortfolioTransactionException>(async () => await Task.WhenAll(task1, task2));
        
        Assert.Equal("Portfolio has already been updated by another entity", exception.Message);
        _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.AtLeast(2));
        _unitOfWorkMock.Verify(u => u.RollbackAsync(), Times.Once);
    }
    
    [Fact]
    public async Task SellStockAsync_Concurrency_Test_Fail()
    {
        const long portfolioId = 1L;
        var transactionRequest = new PortfolioTransactionRequestDto { Symbol = "AAPL", Quantity = 1 };
        var stockData = new StockDataDto { Symbol = "AAPL", Name = "Test AAPL", Close = 150m, Currency = "USD" };
        var portfolio = new Portfolio { Id = portfolioId, CashBalance = 100m };
        var existingHolding = new PortfolioHolding { PortfolioId = portfolioId, StockId = 1, Quantity = 6 };
        var updatedHolding = new PortfolioHolding { PortfolioId = portfolioId, StockId = 1, Quantity = 5 };
        
        var transaction = new PortfolioTransaction
        {
            StockPriceAtTransaction = stockData.Close,
            PortfolioId = portfolioId,
            TransactionType = TransactionTypeEnum.Sell.ToString(),
            Quantity = transactionRequest.Quantity,
            StockId = stockData.Id
        };
        
        _portfolioRepositoryMock.Setup(r => r.FindPortfolioById(portfolioId)).ReturnsAsync(portfolio);

        _stockDbServiceMock.Setup(s => s.GetStockAsync("AAPL")).ReturnsAsync(stockData);

        _portfolioTransactionRepositoryMock.Setup(t => t.CreateTransaction(transaction)).ReturnsAsync((PortfolioTransaction?)null);

        _portfolioRepositoryMock.Setup(r => r.FindPortfolioHoldingByPortfolioId(portfolioId, stockData.Id)).ReturnsAsync(existingHolding);

        _portfolioRepositoryMock.Setup(r => r.SavePortfolioHoldingAsync(It.IsAny<PortfolioHolding>())).ReturnsAsync(updatedHolding);

        _unitOfWorkMock.SetupSequence(u => u.CommitAsync()).Throws(new DbUpdateConcurrencyException()).Returns(Task.CompletedTask); 
        
        var task1 = Task.Run(() => _service.SellStockAsync(transactionRequest, portfolioId));
        var task2 = Task.Run(() => _service.SellStockAsync(transactionRequest, portfolioId));

        var exception =
            await Assert.ThrowsAsync<PortfolioTransactionException>(async () => await Task.WhenAll(task1, task2));
        
        Assert.Equal("Portfolio has already been updated by another entity", exception.Message);
        _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.AtLeast(2));
        _unitOfWorkMock.Verify(u => u.RollbackAsync(), Times.Once);
    }
}