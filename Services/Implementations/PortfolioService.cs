using AutoMapper;
using StockTradingApplication.DTOs;
using StockTradingApplication.Entities;
using StockTradingApplication.Repository.Interfaces;
using StockTradingApplication.Services.Interfaces;

namespace StockTradingApplication.Services.Implementations;

public class PortfolioService(
    IMapper mapper,
    IPortfolioRepository portfolioRepository,
    IUserRepository userRepository) : IPortfolioService
{
    public async Task<PortfolioResponseDto> CreatePortfolioAsync(CreatePortfolioRequestDto createPortfolioRequest)
    {
        var user = await userRepository.GetUserByIdAsync(createPortfolioRequest.UserId);

        var existingPortfolio = await portfolioRepository.GetPortfolioAsync(createPortfolioRequest.UserId);

        ValidationService.ValidateCreatePortfolio(user!, existingPortfolio!);

        var portfolio = mapper.Map<Portfolio>(createPortfolioRequest);
        portfolio.CashBalance = 0m;

        var savedPortfolio = await portfolioRepository.SavePortfolioAsync(portfolio);
        
        return mapper.Map<PortfolioResponseDto>(savedPortfolio);
    }

    public Task DeletePortfolioAsync(long portfolioId)
    {
        throw new NotImplementedException();
    }
}