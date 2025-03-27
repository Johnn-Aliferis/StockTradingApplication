using AutoMapper;
using Moq;
using StockTradingApplication.DTOs;
using StockTradingApplication.Entities;
using StockTradingApplication.Repository.Interfaces;
using StockTradingApplication.Services.Implementations;
using StockTradingApplication.Services.Interfaces;
using Xunit.Abstractions;

namespace StockTradingApplication.Tests;

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
        _userRepositoryMock.Setup(res => res.GetUserByIdAsync(1)).ReturnsAsync(mockedUser);
        _portfolioRepositoryMock.Setup(req => req.GetPortfolioByUserIdAsync(portfolioRequest.UserId)).ReturnsAsync(existingPortfolio);
        _mapperMock.Setup(m => m.Map<Portfolio>(It.IsAny<PortfolioRequestDto>())).Returns(portfolio);
        _portfolioRepositoryMock.Setup(r => r.SavePortfolioAsync(It.IsAny<Portfolio>())).ReturnsAsync(portfolio);
        _mapperMock.Setup(m => m.Map<PortfolioResponseDto>(It.IsAny<Portfolio>())).Returns(portfolioResponse);
        
        var result = await _portfolioService.CreatePortfolioAsync(portfolioRequest);
        Assert.NotNull(result);
        Assert.Equal(0, result.CashBalance);
        _portfolioRepositoryMock.Verify(r => r.SavePortfolioAsync(It.IsAny<Portfolio>()), Times.Once);
        
        _output.WriteLine(" CreatePortfolio_Test_Success PASSED SUCCESSFULLY!");
    }
}