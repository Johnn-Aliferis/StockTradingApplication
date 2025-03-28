using AutoMapper;
using StockTradingApplication.DTOs;
using StockTradingApplication.Entities;

namespace StockTradingApplication.Profiles;

public class PortfolioHoldingProfile : Profile
{
    public PortfolioHoldingProfile()
    {
        CreateMap<PortfolioHolding, PortfolioHoldingResponseDto>()
            .ForMember(h => h.StockQuantity,
                opt => opt.MapFrom(src => src.Quantity))
            .ReverseMap()
            .ForMember(res => res.Quantity,
                opt => opt.MapFrom(src => src.StockQuantity));
    }
}