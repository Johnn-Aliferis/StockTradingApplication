using AutoMapper;
using StockTradingApplication.DTOs;
using StockTradingApplication.Entities;

namespace StockTradingApplication.Profiles;

public class StockHistoryProfile : Profile
{
    public StockHistoryProfile()
    {
        CreateMap<StockHistory, StockHistoryDto>().ReverseMap();
    }
}