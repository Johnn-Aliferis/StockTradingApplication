using Microsoft.AspNetCore.Mvc;
using StockTradingApplication.DTOs;
using StockTradingApplication.Repository.Interfaces;
using StockTradingApplication.Services.Interfaces;

namespace StockTradingApplication.Services.Implementations;

public class PortfolioTransactionServiceService(IUnitOfWork unitOfWork, IPortfolioRepository portfolioRepository) : IPortfolioTransactionService
{
    public Task<IActionResult> BuyStockAsync(PortfolioTransactionRequestDto portfolioTransactionRequestDto)
    {
        ValidationService.ValidateTransactionRequestInput(portfolioTransactionRequestDto);
        throw new NotImplementedException();
    }

    public Task<IActionResult> SellStockAsync(PortfolioTransactionRequestDto portfolioTransactionRequestDto)
    {
        ValidationService.ValidateTransactionRequestInput(portfolioTransactionRequestDto);
        throw new NotImplementedException();
    }
    
    
    // TODO : Algorithm for locking for selling/buying :
    //      1) Utility Validation --> Request is correct
    //      2) Business Validation --> Portfolio existence , balance in portfolio is suffiecient depending on action etc
    //      3) Begin Transaction 
    //      4) Modify and call all repositories needed
    //      5) Attempt to Save Changes --> RowVersion check for optimistic locking
    //      6) If error , rollback and send message appropriate to user
    //      7) If all ok , Save changes in DB and notify User.
}