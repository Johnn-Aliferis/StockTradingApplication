using AutoMapper;
using StockTradingApplication.DTOs;
using StockTradingApplication.Entities;
using StockTradingApplication.Exceptions;

namespace StockTradingApplication.Services;

public class UserService(IUserRepository userRepository, IMapper mapper) : IUserService
{
    private const string UserExists = "User already exists";

    public async Task<AppUser> CreateUserAsync(CreateUserDto createUserDto)
    {
        var user = await userRepository.GetUserAsync(createUserDto.Username);

        if (user != null)
        {
            throw new ValidationException(UserExists);
        }

        var userCreated = mapper.Map<AppUser>(createUserDto);

        return await userRepository.SaveUserAsync(userCreated);
    }
}