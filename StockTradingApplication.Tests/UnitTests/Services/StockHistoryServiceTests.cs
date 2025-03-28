using AutoMapper;
using Moq;
using StockTradingApplication.DTOs;
using StockTradingApplication.Entities;
using StockTradingApplication.Repository.Interfaces;
using StockTradingApplication.Services.Implementations;
using Xunit.Abstractions;

namespace StockTradingApplication.Tests.UnitTests.Services;

public class StockHistoryServiceTests
{
    private readonly StockHistoryService _stockHistoryService;
    private readonly Mock<IStockHistoryRepository> _stockHistoryRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly ITestOutputHelper _output;

    public StockHistoryServiceTests(ITestOutputHelper output)
    {
        _output = output;
        _stockHistoryRepositoryMock = new Mock<IStockHistoryRepository>();
        _mapperMock = new Mock<IMapper>();
        _stockHistoryService = new StockHistoryService(_stockHistoryRepositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task GetStockHistoryAsync_Test_Success()
    {
        const string symbol = "AAPL";
        var date = DateTime.UtcNow;
        var stockHistoryList = new List<StockHistory>
        {
            new StockHistory { Id = 1, Price = 150m },
            new StockHistory { Id = 2, Price = 155m }
        };

        var stockHistoryDtoList = new List<StockHistoryDto>
        {
            new StockHistoryDto { Price = 150m, CreatedAt = date },
            new StockHistoryDto { Price = 155m, CreatedAt = date }
        };

        _stockHistoryRepositoryMock.Setup(repo => repo.GetStockHistory(symbol)).ReturnsAsync(stockHistoryList);

        _mapperMock.Setup(mapper => mapper.Map<List<StockHistoryDto>>(stockHistoryList)).Returns(stockHistoryDtoList);

        var result = await _stockHistoryService.GetStockHistoryAsync(symbol);

        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal(150m, result[0].Price);
        Assert.Equal(155m, result[1].Price);

        _output.WriteLine("GetStockHistoryAsync_Test_Success PASSED SUCCESSFULLY!");
    }
}