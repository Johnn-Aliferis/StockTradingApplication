using StockTradingApplication.Entities;

namespace StockTradingApplication.Repository.Interfaces;

public interface IUserRepository
{
    Task<AppUser?> GetUserAsync(string userId);

    Task<AppUser> SaveUserAsync(AppUser user);
}