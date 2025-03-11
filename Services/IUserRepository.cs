using StockTradingApplication.Entities;

namespace StockTradingApplication.Services;

public interface IUserRepository
{
    Task<AppUser?> GetUserAsync(string userId);

    Task<AppUser> SaveUserAsync(AppUser user);
}