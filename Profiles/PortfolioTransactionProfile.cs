using AutoMapper;
using StockTradingApplication.DTOs;
using StockTradingApplication.Entities;

namespace StockTradingApplication.Profiles;

public class PortfolioTransactionProfile : Profile
{
    public PortfolioTransactionProfile()
    {
        CreateMap<PortfolioTransaction, PortfolioTransactionResponseDto>();
    }
}