using AutoMapper;
using StockTradingApplication.DTOs;
using StockTradingApplication.Entities;
using StockTradingApplication.Repository.Interfaces;
using StockTradingApplication.Services.Interfaces;

namespace StockTradingApplication.Services.Implementations;

public class UserService(IUserRepository userRepository, IMapper mapper) : IUserService
{
    public async Task<AppUserResponseDto> CreateUserAsync(CreateUserRequestDto createUserRequestDto)
    {
        var user = await userRepository.GetUserAsync(createUserRequestDto.Username);

        ValidationService.ValidateCreateUser(user!);

        var userToCreate = mapper.Map<AppUser>(createUserRequestDto);

        var userCreated = await userRepository.SaveUserAsync(userToCreate);
        
        return mapper.Map<AppUserResponseDto>(userCreated);
    }
}