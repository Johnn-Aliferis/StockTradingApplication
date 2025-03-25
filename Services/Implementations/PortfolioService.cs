using System.Collections;
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
    IStockDbService stockDbService,
    IPortfolioRepository portfolioRepository,
    IUserRepository userRepository) : IPortfolioService
{
    public async Task<PortfolioResponseDto> CreatePortfolioAsync(PortfolioRequestDto createPortfolioRequest)
    {
        var user = await userRepository.GetUserByIdAsync(createPortfolioRequest.UserId);

        var existingPortfolio = await portfolioRepository.GetPortfolioByUserIdAsync(createPortfolioRequest.UserId);

        ValidationService.ValidateCreatePortfolio(user!, existingPortfolio!);

        var portfolio = mapper.Map<Portfolio>(createPortfolioRequest);
        portfolio.CashBalance = 0m;

        var savedPortfolio = await portfolioRepository.SavePortfolioAsync(portfolio);

        return mapper.Map<PortfolioResponseDto>(savedPortfolio);
    }

    public async Task<PortfolioResponseDto> AddPortfolioBalance(PortfolioRequestDto portfolioRequest, long porfolioId)
    {
        var existingPortfolio = await portfolioRepository.FindPortfolioById(porfolioId);

        ValidationService.ValidateUpdateCashBalance(existingPortfolio!);

        existingPortfolio!.CashBalance += portfolioRequest.CashBalance;

        var savedPortfolio = await portfolioRepository.SavePortfolioAsync(existingPortfolio);

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

    public async Task<List<PortfolioHoldingResponseDto>> GetPortfolioHoldingsByPortfolioIdAsync(long portfolioId)
    {
        var holdings = await portfolioRepository.FindPortfolioHoldingByPortfolioId(portfolioId);
        return mapper.Map<List<PortfolioHoldingResponseDto>>(holdings);
    }

    public async Task<decimal> CalculateHoldingsBalance(long portfolioId)
    {
        var existingPortfolio = await portfolioRepository.FindPortfolioById(portfolioId);

        if (existingPortfolio is null)
        {
            throw new PortfolioException($"Portfolio with ID {portfolioId} not found", HttpStatusCode.NotFound);
        }
        
        var holdings = await portfolioRepository.FindPortfolioHoldingByPortfolioId(portfolioId);

        var valueTasks = holdings.Select(async holding =>
        {
            var stock = await stockDbService.GetStockAsync(holding.Stock.Symbol);
            return holding.Quantity * stock!.Close;
        });

        var individualValues = await Task.WhenAll(valueTasks);

        return individualValues.Sum();
    }
}