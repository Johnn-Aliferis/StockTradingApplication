using StockTradingApplication.Entities;

namespace StockTradingApplication.Services;

public interface IUserRepository
{
    Task<User?> GetUserAsync(string userId);

    Task<User> SaveUserAsync(User user);
}