using AutoMapper;
using Moq;
using StockTradingApplication.DTOs;
using StockTradingApplication.Entities;
using StockTradingApplication.Exceptions;
using StockTradingApplication.Repository.Interfaces;
using StockTradingApplication.Services.Implementations;
using StockTradingApplication.Services.Interfaces;
using Xunit.Abstractions;

namespace StockTradingApplication.Tests.UnitTests.Services;

public class PortfolioServiceTests
{
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IStockDbService> _stockDbServiceMock;
    private readonly Mock<IPortfolioRepository> _portfolioRepositoryMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly PortfolioService _portfolioService;
    private readonly ITestOutputHelper _output;
    
    
    public PortfolioServiceTests(ITestOutputHelper output)
    {
        _output = output;
        _mapperMock = new Mock<IMapper>();
        _stockDbServiceMock = new Mock<IStockDbService>();
        _portfolioRepositoryMock = new Mock<IPortfolioRepository>();
        _userRepositoryMock = new Mock<IUserRepository>();

        _portfolioService = new PortfolioService(
            _mapperMock.Object,
            _stockDbServiceMock.Object,
            _portfolioRepositoryMock.Object,
            _userRepositoryMock.Object
        );
    }
    
    [Fact]
    public async Task CreatePortfolio_Test_Success()
    {
        var date = DateTime.UtcNow;
        var portfolioRequest = new PortfolioRequestDto { UserId = 1 };
        var mockedUser = new AppUser { Id = 1, Username = "test username" };
        Portfolio? existingPortfolio = null; 

        var portfolio = new Portfolio { Id = 1, CashBalance = 0 };
        var portfolioResponse = new PortfolioResponseDto { Id = 1, CashBalance = 0, UserId = 1, CreatedAt = date ,UpdatedAt = date};

        // Mock
        _userRepositoryMock.Setup(rep => rep.GetUserByIdAsync(1)).ReturnsAsync(mockedUser);
        _portfolioRepositoryMock.Setup(rep => rep.GetPortfolioByUserIdAsync(portfolioRequest.UserId)).ReturnsAsync(existingPortfolio);
        _mapperMock.Setup(m => m.Map<Portfolio>(It.IsAny<PortfolioRequestDto>())).Returns(portfolio);
        _portfolioRepositoryMock.Setup(r => r.SavePortfolioAsync(It.IsAny<Portfolio>())).ReturnsAsync(portfolio);
        _mapperMock.Setup(m => m.Map<PortfolioResponseDto>(It.IsAny<Portfolio>())).Returns(portfolioResponse);
        
        var result = await _portfolioService.CreatePortfolioAsync(portfolioRequest);
        Assert.NotNull(result);
        Assert.Equal(0, result.CashBalance);
        _portfolioRepositoryMock.Verify(r => r.SavePortfolioAsync(It.IsAny<Portfolio>()), Times.Once);
        
        _output.WriteLine(" CreatePortfolio_Test_Success PASSED SUCCESSFULLY!");
    }
    
    [Fact]
    public async Task CreatePortfolio_Test_UserNotFound()
    {
        var portfolioRequest = new PortfolioRequestDto { UserId = 1 };
        AppUser? mockedUser = null;
        Portfolio? existingPortfolio = null; 

        // Mock
        _userRepositoryMock.Setup(rep => rep.GetUserByIdAsync(It.IsAny<int>())).ReturnsAsync(mockedUser);
        _portfolioRepositoryMock.Setup(rep => rep.GetPortfolioByUserIdAsync(It.IsAny<int>())).ReturnsAsync(existingPortfolio);
        
        var exception = await Assert.ThrowsAsync<ValidationException>(async () => 
            await _portfolioService.CreatePortfolioAsync(portfolioRequest)
        );
        
        Assert.Equal("The provided user id does not match to a registered user", exception.Message);
        
        _output.WriteLine(" CreatePortfolio_Test_UserNotFound PASSED SUCCESSFULLY!");
    }
    
    [Fact]
    public async Task CreatePortfolio_Test_PortfolioNotFound()
    {
        var date = DateTime.UtcNow;
        var portfolioRequest = new PortfolioRequestDto { UserId = 1 };
        var mockedUser = new AppUser { Id = 1, Username = "test username" };
        var existingPortfolio = new Portfolio {Id = 1, UserId = mockedUser.Id ,CashBalance = 0m , UpdatedAt = date, CreatedAt = date} ; 

        // Mock
        _userRepositoryMock.Setup(rep => rep.GetUserByIdAsync(mockedUser.Id)).ReturnsAsync(mockedUser);
        _portfolioRepositoryMock.Setup(rep => rep.GetPortfolioByUserIdAsync(portfolioRequest.UserId)).ReturnsAsync(existingPortfolio);
        
        var exception = await Assert.ThrowsAsync<ValidationException>(async () => 
            await _portfolioService.CreatePortfolioAsync(portfolioRequest)
        );
        
        Assert.Equal("A Portfolio for this user already exists", exception.Message);
        
        _output.WriteLine(" CreatePortfolio_Test_PortfolioNotFound PASSED SUCCESSFULLY!");
    }
    
    
    [Fact]
    public async Task AddPortfolioBalance_Test_Success()
    {
        var date = DateTime.UtcNow;
        
        var request = new PortfolioRequestDto { UserId = 1 , CashBalance = 100m };
        var response = new PortfolioResponseDto { Id = 1, CashBalance = 100m, UserId = 1, CreatedAt = date ,UpdatedAt = date};
        var existingPortfolio = new Portfolio {Id = 1, UserId = request.UserId ,CashBalance = 0m , UpdatedAt = date, CreatedAt = date}; 
        var updatedPortfolio = new Portfolio {Id = 1, UserId = request.UserId ,CashBalance = 100m , UpdatedAt = date, CreatedAt = date}; 
        

        // Mock
        _portfolioRepositoryMock.Setup(rep => rep.FindPortfolioById(request.UserId)).ReturnsAsync(existingPortfolio);
        _portfolioRepositoryMock.Setup(rep => rep.SavePortfolioAsync(It.IsAny<Portfolio>())).ReturnsAsync(updatedPortfolio);
        _mapperMock.Setup(m => m.Map<PortfolioResponseDto>(It.IsAny<Portfolio>())).Returns(response);
        
        var result = await _portfolioService.AddPortfolioBalance(request, 1);
        Assert.NotNull(result);
        Assert.Equal(100m, updatedPortfolio.CashBalance);
        _portfolioRepositoryMock.Verify(r => r.SavePortfolioAsync(It.IsAny<Portfolio>()), Times.Once);
        
        _output.WriteLine(" AddPortfolioBalance_Test_Success PASSED SUCCESSFULLY!");
    }
    
    [Fact]
    public async Task AddPortfolioBalance_Test_Failure()
    {
        const long portfolioId = 1;
        var request = new PortfolioRequestDto { UserId = 1 , CashBalance = 100m };
        Portfolio? existingPortfolio = null; 
        
        // Mock
        _portfolioRepositoryMock.Setup(rep => rep.FindPortfolioById(portfolioId)).ReturnsAsync(existingPortfolio);
        
        var exception = await Assert.ThrowsAsync<ValidationException>(async () => 
            await _portfolioService.AddPortfolioBalance(request, portfolioId)
        );
        
        Assert.Equal("The id provided does not correspond to a Portfolio", exception.Message);
        
        _output.WriteLine(" AddPortfolioBalance_Test_Failure PASSED SUCCESSFULLY!");
    }
    
    [Fact]
    public async Task DeletePortfolio_Test_Failure()
    {
        const long portfolioId = 1;
        Portfolio? existingPortfolio = null;  
        
        // Mock
        _portfolioRepositoryMock.Setup(rep => rep.FindPortfolioById(portfolioId)).ReturnsAsync(existingPortfolio);
        
        var exception = await Assert.ThrowsAsync<PortfolioException>(async () => 
            await _portfolioService.DeletePortfolioAsync(portfolioId)
        );
        
        Assert.Equal($"Portfolio with ID {portfolioId} not found", exception.Message);
        
        _output.WriteLine(" DeletePortfolio_Test_Failure PASSED SUCCESSFULLY!");
    }

    [Fact]
    public async Task DeletePortfolio_Test_Success()
    {
        var date = DateTime.UtcNow;
        const long portfolioId = 1;
        var existingPortfolio = new Portfolio {Id = 1, UserId = 1 ,CashBalance = 0m , UpdatedAt = date, CreatedAt = date}; 
        
        // Mock
        _portfolioRepositoryMock.Setup(rep => rep.FindPortfolioById(portfolioId)).ReturnsAsync(existingPortfolio);
        
        await _portfolioService.DeletePortfolioAsync(portfolioId);
        _portfolioRepositoryMock.Verify(repo => repo.DeletePortfolioAsync(existingPortfolio), Times.Once);
        
        _output.WriteLine(" DeletePortfolio_Test_Success PASSED SUCCESSFULLY!");
    }
    
    [Fact]
    public async Task CalculateHoldingsBalance_Test_Success()
    {
        var date = DateTime.UtcNow;
        const long portfolioId = 1;
        const decimal expectedTotal = (10 * 15m) + (5 * 25m);
        var existingPortfolio = new Portfolio {Id = 1, UserId = 1 ,CashBalance = 0m , UpdatedAt = date, CreatedAt = date}; 
        var holdings = new List<PortfolioHolding>
        {
            new PortfolioHolding { Stock = new Stock { Symbol = "AAPL" }, Quantity = 10 },
            new PortfolioHolding { Stock = new Stock { Symbol = "MSFT" }, Quantity = 5 }
        };
        
        // Mock
        _portfolioRepositoryMock.Setup(rep => rep.FindPortfolioById(portfolioId)).ReturnsAsync(existingPortfolio);
        _portfolioRepositoryMock.Setup(rep => rep.FindPortfolioHoldingByPortfolioId(portfolioId)).ReturnsAsync(holdings);
        _stockDbServiceMock.Setup(service => service.GetStockAsync("AAPL"))
            .ReturnsAsync(new StockDataDto() { Symbol = "AAPL", Name = "Test AAPL", Close = 15m, Currency = "USD"});
        _stockDbServiceMock.Setup(service => service.GetStockAsync("MSFT"))
            .ReturnsAsync(new StockDataDto() { Symbol = "MSFT", Name = "Test MSFT", Close = 25m, Currency = "USD"});
        
        var result = await _portfolioService.CalculateHoldingsBalance(portfolioId);
        Assert.Equal(expectedTotal, result);
        
        _output.WriteLine(" CalculateHoldingsBalance_Test_Success PASSED SUCCESSFULLY!");
    }
    
    [Fact]
    public async Task CalculateHoldingsBalance_Test_Failure_Portfolio_NotFound()
    {
        const long portfolioId = 1;
        Portfolio? existingPortfolio = null;  
        
        // Mock
        _portfolioRepositoryMock.Setup(rep => rep.FindPortfolioById(portfolioId)).ReturnsAsync(existingPortfolio);
        
        var exception = await Assert.ThrowsAsync<PortfolioException>(async () => 
            await _portfolioService.CalculateHoldingsBalance(portfolioId)
        );
        
        Assert.Equal($"Portfolio with ID {portfolioId} not found", exception.Message);
        _output.WriteLine(" CalculateHoldingsBalance_Test_Failure_Portfolio_NotFound PASSED SUCCESSFULLY!");
    }
}