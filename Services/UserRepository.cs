using Microsoft.EntityFrameworkCore;
using StockTradingApplication.Entities;
using StockTradingApplication.Persistence;

namespace StockTradingApplication.Services;

public class UserRepository(AppDbContext context) : IUserRepository
{
    private readonly DbSet<AppUser> _user = context.Set<AppUser>();
    
    public async Task<AppUser?> GetUserAsync(string username)
    {
        return await _user.FirstOrDefaultAsync(user => user.Username == username);
    }

    public async Task<AppUser> SaveUserAsync(AppUser user)
    {
        var createdUser = await _user.AddAsync(user);
        await context.SaveChangesAsync();
        return createdUser.Entity; 
    }
}