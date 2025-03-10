using System.ComponentModel.DataAnnotations;
using StockTradingApplication.DTOs;
using StockTradingApplication.Entities;
using StockTradingApplication.Mappers;

namespace StockTradingApplication.Services;

public class UserService(IUserRepository userRepository) : IUserService
{
    private const string UserExists = "User already exists";

    public async Task<User> CreateUserAsync(CreateUserDto createUserDto)
    {
        var user = await userRepository.GetUserAsync(createUserDto.Username);

        if (user != null)
        {
            throw new ValidationException(UserExists);
        }

        var userCreated = UserMapper.ToUserEntity(createUserDto);

        return await userRepository.SaveUserAsync(userCreated);
    }
}