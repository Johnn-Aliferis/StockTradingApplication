using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using StockTradingApplication.Common;
using StockTradingApplication.Entities;
using StockTradingApplication.Persistence;

namespace StockTradingApplication.Tests.IntegrationTests;

public class PortfolioCascadeDeleteTests(TestContainers containers) : IClassFixture<TestContainers>
{
    
    private readonly AppDbContext _dbContext = containers.DbContext;

    [Fact]
    public async Task DeletingPortfolio_ShouldCascadeDelete_Holdings()
    {
        var stocks = _dbContext.Set<Stock>();
        var users = _dbContext.Set<AppUser>();
        var portfolios = _dbContext.Set<Portfolio>();
        var portfoliosHoldings = _dbContext.Set<PortfolioHolding>();
        var portfoliosTransactions = _dbContext.Set<PortfolioTransaction>();

        var user = new AppUser { Id = 1, Username = "Test-Username"};
        var stock = new Stock { Id = 1, Symbol = "Test-Symbol" , Currency = "Test-Currency" , Name = "Test-Name" , Price = 400m, CreatedAt = DateTime.UtcNow };
        var portfolio = new Portfolio { Id = 1, CashBalance = 1000m, UserId = 1};
        var holding = new PortfolioHolding { Id = 1, PortfolioId = 1, StockId = 1, Quantity = 5 };
        var transaction = new PortfolioTransaction
        {
            Id = 1, PortfolioId = 1, StockId = 1, Quantity = 5, TransactionType = TransactionTypeEnum.Buy.ToString()
        };

        users.Add(user);
        stocks.Add(stock);
        portfolios.Add(portfolio);
        portfoliosHoldings.Add(holding);
        portfoliosTransactions.Add(transaction);

        await _dbContext.SaveChangesAsync();

        portfolios.Remove(portfolio);
        await _dbContext.SaveChangesAsync();

        // Portfolio Holdings should also be deleted
        var holdingsAfterDelete = await portfoliosHoldings.ToListAsync();
        holdingsAfterDelete.Should().BeEmpty();

        // Portfolio Transactions should also be deleted
        var transactionsAfterDelete = await portfoliosTransactions.ToListAsync();
        transactionsAfterDelete.Should().BeEmpty();
    }
}