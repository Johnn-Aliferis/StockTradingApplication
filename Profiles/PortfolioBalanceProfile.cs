using AutoMapper;
using StockTradingApplication.DTOs;
using StockTradingApplication.Entities;

namespace StockTradingApplication.Profiles;

public class PortfolioBalanceProfile : Profile
{
    public PortfolioBalanceProfile()
    {
        CreateMap<PortfolioBalance, PortfolioBalanceResponseDto>().ReverseMap();
    }
}