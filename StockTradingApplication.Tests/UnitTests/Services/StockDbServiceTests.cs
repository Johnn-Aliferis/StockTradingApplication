using AutoMapper;
using Moq;
using StockTradingApplication.DTOs;
using StockTradingApplication.Entities;
using StockTradingApplication.Repository.Interfaces;
using StockTradingApplication.Services.Implementations;
using Xunit.Abstractions;

namespace StockTradingApplication.Tests.UnitTests.Services;

public class StockDbServiceTests
{
    private readonly Mock<IStockRepository> _stockRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly StockDbService _stockDbService;
    private readonly ITestOutputHelper _output;
    
    public StockDbServiceTests(ITestOutputHelper output)
    {
        _output = output;
        _stockRepositoryMock = new Mock<IStockRepository>();
        _mapperMock = new Mock<IMapper>();

        _stockDbService = new StockDbService(_stockRepositoryMock.Object, _mapperMock.Object);
    }
    
    [Fact]
    public async Task HandleExternalProviderData_Success()
    {
        var dateNow = DateTime.UtcNow;
        
        var externalStockData = new List<StockDataDto>
        {
            new StockDataDto { Symbol = "AAPL", Name = "Apple", Close = 15m, Currency = "USD" },
            new StockDataDto { Symbol = "GOOGL", Name = "Google", Close = 28m, Currency = "USD" }
        };

        var expectedMergedData = new List<Stock>
        {
            new Stock { Symbol = "AAPL", Name = "Apple", Price = 16m, Currency = "USD" },
            new Stock { Symbol = "GOOGL", Name = "Google", Price = 28.5m, Currency = "USD" }
        };
        
        var resultReturned = new List<StockDataDto>
        {
            new StockDataDto { Symbol = "AAPL", Name = "Apple", Close = 16m, Currency = "USD" },
            new StockDataDto { Symbol = "GOOGL", Name = "Google", Close = 28.5m, Currency = "USD" }
        };
        
        _stockRepositoryMock.Setup(repo => repo.HandleInsertAndUpdateBulkOperationAsync(It.IsAny<SqlQueryDto>(),
                It.IsAny<SqlQueryDto>())).ReturnsAsync(expectedMergedData);
        _mapperMock.Setup(m => m.Map<List<StockDataDto>>(expectedMergedData)).Returns(resultReturned);
        
        var result = await _stockDbService.HandleExternalProviderData(externalStockData);
        
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal("AAPL", result[0].Symbol);
        Assert.Equal(16m, result[0].Close);
        Assert.Equal("GOOGL", result[1].Symbol);
        Assert.Equal(28.5m, result[1].Close);
        
        _stockRepositoryMock.Verify(repo => repo.HandleInsertAndUpdateBulkOperationAsync(It.IsAny<SqlQueryDto>(), 
            It.IsAny<SqlQueryDto>()), Times.Once);
        _mapperMock.Verify(m => m.Map<List<StockDataDto>>(expectedMergedData), Times.Once);
        
        _output.WriteLine("HandleExternalProviderData_Success PASSED SUCCESSFULLY!");
    }
    
}