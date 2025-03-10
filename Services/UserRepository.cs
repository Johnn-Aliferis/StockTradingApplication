using Microsoft.EntityFrameworkCore;
using StockTradingApplication.Entities;
using StockTradingApplication.Persistence;

namespace StockTradingApplication.Services;

public class UserRepository(AppDbContext context) : IUserRepository
{
    private readonly DbSet<User> _user = context.Set<User>();
    
    public async Task<User?> GetUserAsync(string username)
    {
        return await _user.FirstOrDefaultAsync(user => user.Username == username);
    }

    public async Task<User> SaveUserAsync(User user)
    {
        var createdUser = await _user.AddAsync(user);
        await context.SaveChangesAsync();
        return createdUser.Entity; 
    }
}