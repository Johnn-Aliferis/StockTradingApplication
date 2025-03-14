using StockTradingApplication.DTOs;

namespace StockTradingApplication.Services.Interfaces;

public interface IUserService
{
    Task<AppUserResponseDto> CreateUserAsync(CreateUserRequestDto createUserRequestDto);
}