using System.ComponentModel.DataAnnotations;
using StockTradingApplication.DTOs;
using StockTradingApplication.Entities;
using StockTradingApplication.Mappers;
using StockTradingApplication.Exceptions;

namespace StockTradingApplication.Services;

public class UserService(IUserRepository userRepository) : IUserService
{
    private const string UserExists = "User already exists";

    public async Task<AppUser> CreateUserAsync(CreateUserDto createUserDto)
    {
        var user = await userRepository.GetUserAsync(createUserDto.Username);

        if (user != null)
        {
            throw new Exceptions.ValidationException(UserExists);
        }

        var userCreated = UserMapper.ToUserEntity(createUserDto);

        return await userRepository.SaveUserAsync(userCreated);
    }
}