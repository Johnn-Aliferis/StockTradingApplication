using StockTradingApplication.Entities;

namespace StockTradingApplication.Repository.Interfaces;

public interface IUserRepository
{
    Task<AppUser?> GetUserAsync(string username);
    Task<List<AppUser>> GetUsersAsync();
    Task<AppUser?> GetUserByIdAsync(long userId);

    Task<AppUser> SaveUserAsync(AppUser user);
}