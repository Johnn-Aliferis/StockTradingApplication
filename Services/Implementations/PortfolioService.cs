using System.Net;
using AutoMapper;
using StockTradingApplication.DTOs;
using StockTradingApplication.Entities;
using StockTradingApplication.Exceptions;
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

    public async Task DeletePortfolioAsync(long portfolioId)
    {
        var existingPortfolio = await portfolioRepository.FindPortfolioById(portfolioId);

        if (existingPortfolio is null)
        {
            throw new PortfolioException($"Portfolio with ID {portfolioId} not found", HttpStatusCode.NotFound);
        }

        await portfolioRepository.DeletePortfolioAsync(existingPortfolio);
    }

    public async Task<PortfolioResponseDto?> GetPortfolioAsync(long portfolioId)
    {
        var existingPortfolio = await portfolioRepository.FindPortfolioById(portfolioId);
        return existingPortfolio is not null ? mapper.Map<PortfolioResponseDto>(existingPortfolio) : null;
    }

    public async Task<PortfolioBalanceResponseDto?> GetPortfolioBalanceAsync(long portfolioId)
    {
        var portfolioBalance = await portfolioRepository.FindPortfolioBalanceByPortfolioId(portfolioId);
        return portfolioBalance is not null ? mapper.Map<PortfolioBalanceResponseDto>(portfolioBalance) : null;
    }
    
    public async Task<PortfolioHoldingResponseDto?> GetPortfolioHoldingAsync(long portfolioId, long stockId)
    {
        var portfolioHolding = await portfolioRepository.FindPortfolioHoldingByPortfolioId(portfolioId, stockId);
        return portfolioHolding is not null ? mapper.Map<PortfolioHoldingResponseDto>(portfolioHolding) : null;
    }
}