using StockTradingApplication.DTOs;
using StockTradingApplication.Entities;

namespace StockTradingApplication.Mappers;

public class UserMapper
{
    public static AppUser ToUserEntity(CreateUserDto createUserDto)
    {
        return new AppUser
        {
            Username = createUserDto.Username
        };
    }
}