using AutoMapper;
using StockTradingApplication.DTOs;
using StockTradingApplication.Entities;

namespace StockTradingApplication.Profiles;

public class PortfolioProfile : Profile
{
    public PortfolioProfile()
    {
        CreateMap<PortfolioRequestDto, Portfolio>()
            .ForMember(dest => dest.CashBalance, opt => opt.Ignore())
            .ForMember(dest => dest.AppUser, opt => opt.Ignore())
            .ForMember(dest => dest.Transactions, opt => opt.Ignore())
            .ForMember(dest => dest.RowVersion, opt => opt.Ignore())
            .ForMember(dest => dest.Holdings, opt => opt.Ignore());

        CreateMap<Portfolio, PortfolioResponseDto>().ReverseMap();
    }
}