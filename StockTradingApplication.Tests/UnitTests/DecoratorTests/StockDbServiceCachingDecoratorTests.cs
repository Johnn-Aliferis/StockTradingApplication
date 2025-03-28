using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Moq;
using StockTradingApplication.Decorators;
using StockTradingApplication.DTOs;
using StockTradingApplication.Services.Interfaces;

namespace StockTradingApplication.Tests.UnitTests.DecoratorTests;

public class StockDbServiceCachingDecoratorTests
{
    private readonly Mock<IStockDbService> _decoratedServiceMock;
    private readonly Mock<IDistributedCache> _cacheMock;
    private readonly Mock<ILogger<StockDbServiceCachingDecorator>> _loggerMock;
    private readonly StockDbServiceCachingDecorator _decorator;
    
    public StockDbServiceCachingDecoratorTests()
    {
        _decoratedServiceMock = new Mock<IStockDbService>();
        _cacheMock = new Mock<IDistributedCache>();
        _loggerMock = new Mock<ILogger<StockDbServiceCachingDecorator>>();

        _decorator = new StockDbServiceCachingDecorator(
            _decoratedServiceMock.Object,
            _cacheMock.Object,
            _loggerMock.Object
        );
    }
    
    [Fact]
    public async Task GetStocksAsync_FromCache_Test()
    {
        const string cacheKey = "stocks";
        var cachedStocks = new List<StockDataDto>
        {
            new StockDataDto { Symbol = "AAPL", Name = "Apple", Close = 150m, Currency = "USD" }
        };
        var cachedJson = JsonSerializer.Serialize(cachedStocks);

        _cacheMock.Setup(c => c.GetAsync("stocks", CancellationToken.None)).ReturnsAsync(System.Text.Encoding.UTF8.GetBytes(cachedJson));
        
        var result = await _decorator.GetStocksAsync();
        
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("AAPL", result[0].Symbol);

        _cacheMock.Verify(c => c.GetAsync(cacheKey, CancellationToken.None), Times.Once);
        _decoratedServiceMock.Verify(s => s.GetStocksAsync(), Times.Never);
    }
    
    [Fact]
    public async Task GetStocksAsync_From_DB_Test()
    {
        const string cacheKey = "stocks";
        var databaseStocks = new List<StockDataDto>
        {
            new StockDataDto { Symbol = "MSFT", Name = "Microsoft", Close = 285m, Currency = "USD" }
        };

        _cacheMock.Setup(c => c.GetAsync(cacheKey, CancellationToken.None)).ReturnsAsync((byte[]?)null);

        _decoratedServiceMock.Setup(s => s.GetStocksAsync()).ReturnsAsync(databaseStocks);
        
        var result = await _decorator.GetStocksAsync();
        
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("MSFT", result[0].Symbol);

        _decoratedServiceMock.Verify(s => s.GetStocksAsync(), Times.Once);
        _cacheMock.Verify(c => c.SetAsync(
                cacheKey, 
                It.IsAny<byte[]>(),
                It.IsAny<DistributedCacheEntryOptions>(), 
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
    
    [Fact]
    public async Task HandleExternalProviderData_Test_Success()
    {
        var stocks = new List<StockDataDto>
        {
            new StockDataDto { Symbol = "AAPL", Name = "Apple", Close = 160m, Currency = "USD" }
        };

        _decoratedServiceMock.Setup(s => s.HandleExternalProviderData(It.IsAny<List<StockDataDto>>())).ReturnsAsync(stocks);
        
        var result = await _decorator.HandleExternalProviderData(stocks);
        
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("AAPL", result[0].Symbol);

        _cacheMock.Verify(c => c.SetAsync(
                "stocks",
                It.IsAny<byte[]>(),
                It.IsAny<DistributedCacheEntryOptions>(), 
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}