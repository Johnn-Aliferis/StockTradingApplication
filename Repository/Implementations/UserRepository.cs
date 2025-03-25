using Microsoft.EntityFrameworkCore;
using StockTradingApplication.Entities;
using StockTradingApplication.Persistence;
using StockTradingApplication.Repository.Interfaces;

namespace StockTradingApplication.Repository.Implementations;

public class UserRepository(AppDbContext context) : IUserRepository
{
    private readonly DbSet<AppUser> _users = context.Set<AppUser>();
    
    public async Task<AppUser?> GetUserAsync(string username)
    {
        return await _users.FirstOrDefaultAsync(user => user.Username == username);
    }

    public async Task<List<AppUser>> GetUsersAsync()
    {
        return await _users.ToListAsync();
    }

    public async Task<AppUser?> GetUserByIdAsync(long userId)
    {
        return await _users.FirstOrDefaultAsync(user => user.Id == userId);
    }

    public async Task<AppUser> SaveUserAsync(AppUser user)
    {
        var createdUser = await _users.AddAsync(user);
        await context.SaveChangesAsync();
        return createdUser.Entity; 
    }
}