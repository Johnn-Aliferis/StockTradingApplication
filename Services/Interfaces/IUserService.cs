using StockTradingApplication.DTOs;
using StockTradingApplication.Entities;

namespace StockTradingApplication.Services.Interfaces;

public interface IUserService
{
    Task<AppUser> CreateUserAsync(CreateUserRequestDto createUserRequestDto);
}