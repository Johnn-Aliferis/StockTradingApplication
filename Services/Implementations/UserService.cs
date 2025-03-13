using AutoMapper;
using StockTradingApplication.DTOs;
using StockTradingApplication.Entities;
using StockTradingApplication.Exceptions;
using StockTradingApplication.Repository.Interfaces;
using StockTradingApplication.Services.Interfaces;

namespace StockTradingApplication.Services.Implementations;

public class UserService(IUserRepository userRepository, IMapper mapper) : IUserService
{
    private const string UserExists = "User already exists";

    public async Task<AppUser> CreateUserAsync(CreateUserRequestDto createUserRequestDto)
    {
        var user = await userRepository.GetUserAsync(createUserRequestDto.Username);

        if (user != null)
        {
            throw new ValidationException(UserExists);
        }

        var userCreated = mapper.Map<AppUser>(createUserRequestDto);

        return await userRepository.SaveUserAsync(userCreated);
    }
}