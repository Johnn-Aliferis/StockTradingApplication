using StockTradingApplication.DTOs;
using StockTradingApplication.Entities;

namespace StockTradingApplication.Services;

public interface IUserService
{
    Task<User> CreateUserAsync(CreateUserDto createUserDto);
}