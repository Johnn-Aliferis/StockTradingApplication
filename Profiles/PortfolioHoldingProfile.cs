using AutoMapper;
using StockTradingApplication.DTOs;
using StockTradingApplication.Entities;

namespace StockTradingApplication.Profiles;

public class PortfolioHoldingProfile : Profile
{
    public PortfolioHoldingProfile()
    {
        CreateMap<PortfolioHolding, PortfolioHoldingResponseDto>().ReverseMap();
    }
}