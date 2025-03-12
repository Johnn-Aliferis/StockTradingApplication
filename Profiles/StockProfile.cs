using AutoMapper;
using StockTradingApplication.DTOs;
using StockTradingApplication.Entities;

namespace StockTradingApplication.Profiles;

public class StockProfile : Profile
{
    public StockProfile()
    {
        CreateMap<Stock, StockDataDto>()
            .ForMember(dest => dest.Close, opt => opt.MapFrom(src => src.Price))
            .ReverseMap()
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Close));
    }
}