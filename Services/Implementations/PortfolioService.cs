using AutoMapper;
using StockTradingApplication.DTOs;
using StockTradingApplication.Entities;
using StockTradingApplication.Exceptions;
using StockTradingApplication.Repository.Interfaces;
using StockTradingApplication.Services.Interfaces;

namespace StockTradingApplication.Services.Implementations;

public class PortfolioService(
    IMapper mapper,
    IPortfolioRepository portfolioRepository) : IPortfolioService
{
    private const string PortfolioExists = "A Portfolio for this user already exists";

    public async Task<Portfolio> CreatePortfolioAsync(CreatePortfolioRequestDto createPortfolioRequest)
    {
        // TODO : Check in testing how it behaves if we pass something random ? I.e meaning the request field is null.
        var existingPortfolio = await portfolioRepository.GetPortfolioAsync(createPortfolioRequest.UserId);

        if (existingPortfolio != null)
        {
            throw new ValidationException(PortfolioExists);
        }

        var portfolio = mapper.Map<Portfolio>(createPortfolioRequest);

        return await portfolioRepository.SavePortfolioAsync(portfolio);
    }

    public Task DeletePortfolioAsync(long portfolioId)
    {
        throw new NotImplementedException();
    }
}