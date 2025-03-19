using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockTradingApplication.Entities;

namespace StockTradingApplication.Configuration;

public class PortfolioConfiguration : IEntityTypeConfiguration<Portfolio>
{
    public void Configure(EntityTypeBuilder<Portfolio> builder)
    {
        // Cascading on delete for portfolio for data integrity.
        
        // Portfolio --> Transactions
        builder.HasMany(p => p.Transactions)
            .WithOne(pt => pt.Portfolio)
            .OnDelete(DeleteBehavior.Cascade);
        
        // Portfolio --> Holdings
        builder.HasMany(p => p.Holdings)
            .WithOne(ph => ph.Portfolio)
            .OnDelete(DeleteBehavior.Cascade);
    }
}