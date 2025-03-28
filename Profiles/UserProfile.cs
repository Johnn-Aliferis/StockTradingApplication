using AutoMapper;
using StockTradingApplication.DTOs;
using StockTradingApplication.Entities;

namespace StockTradingApplication.Profiles;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<CreateUserRequestDto, AppUser>();
        CreateMap<AppUser, AppUserResponseDto>();
    }
}