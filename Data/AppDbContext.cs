using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace StockTradingApplication.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Reflection to dynamically register entities
        foreach (var entityType in Assembly.GetExecutingAssembly()
                     .GetTypes()
                     .Where(t => t is { IsClass: true, IsAbstract: false, Namespace: "StockTradingApplication.Entities" }))
        {
            modelBuilder.Entity(entityType);
        }
    }
}