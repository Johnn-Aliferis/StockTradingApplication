using StockTradingApplication.DTOs;
using StockTradingApplication.Entities;

namespace StockTradingApplication.Mappers;

public class UserMapper
{
    public static User ToUserEntity(CreateUserDto createUserDto)
    {
        return new User
        {
            Username = createUserDto.Username
        };
    }
}