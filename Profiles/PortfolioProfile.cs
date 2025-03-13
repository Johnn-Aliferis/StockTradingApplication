using AutoMapper;
using StockTradingApplication.DTOs;
using StockTradingApplication.Entities;

namespace StockTradingApplication.Profiles;

public class PortfolioProfile : Profile
{
    public PortfolioProfile()
    {
        CreateMap<CreatePortfolioRequestDto, Portfolio>()
            .ForMember(dest => dest.CashBalance, opt => opt.MapFrom(src => 0m))
            .ForMember(dest => dest.AppUser, opt => opt.Ignore())
            .ForMember(dest => dest.Balance, opt => opt.Ignore())
            .ForMember(dest => dest.Transactions, opt => opt.Ignore())
            .ForMember(dest => dest.Holdings, opt => opt.Ignore());   
    }
}