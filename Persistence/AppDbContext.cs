using System.Reflection;
using Microsoft.EntityFrameworkCore;
using StockTradingApplication.Entities;

namespace StockTradingApplication.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Reflection to dynamically register entities
        foreach (var entityType in Assembly.GetExecutingAssembly()
                     .GetTypes()
                     .Where(t => t is { IsClass: true, IsAbstract: false, Namespace: "StockTradingApplication.Entities" } && t.Name != "BaseEntity"))
        {
            modelBuilder.Entity(entityType);
        }
    }
    
    // For each entity modified , update updated_at column to date time now.utc
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = DateTime.UtcNow;
            }
        }
        return await base.SaveChangesAsync(cancellationToken);
    }
}