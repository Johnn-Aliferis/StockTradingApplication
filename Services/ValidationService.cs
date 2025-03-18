using StockTradingApplication.DTOs;
using StockTradingApplication.Entities;
using StockTradingApplication.Exceptions;

namespace StockTradingApplication.Services;

public class ValidationService
{
    private const string BuyStock = "buy";
    private const string SellStock = "sell";

    private const string UserExists = "User already exists";
    private const string PortfolioExists = "A Portfolio for this user already exists";
    private const string UserDoesNotExist = "The provided id does not match to a registered user";
    private const string InvalidTransactionRequest = "Invalid Transaction Request";
    private const string InvalidTransactionRequestQuantity = "Stock Quantity is invalid";
    private const string InvalidTransactionRequestAction = "Invalid Action Received";
    private const string InvalidTransactionRequestSymbol = "Invalid Symbol Received";

    public static void ValidateCreatePortfolio(AppUser user, Portfolio portfolio)
    {
        if (user is null)
        {
            throw new ValidationException(UserDoesNotExist);
        }

        if (portfolio is not null)
        {
            throw new ValidationException(PortfolioExists);
        }
    }

    public static void ValidateCreateUser(AppUser user)
    {
        if (user is not null)
        {
            throw new ValidationException(UserExists);
        }
    }

    public static void ValidateTransactionRequestInput(PortfolioTransactionRequestDto portfolioTransactionRequestDto)
    {
        if (portfolioTransactionRequestDto is null)
        {
            throw new ValidationException(InvalidTransactionRequest);
        }

        if (portfolioTransactionRequestDto.Quantity <= 0)
        {
            throw new ValidationException(InvalidTransactionRequestQuantity);
        }

        if (portfolioTransactionRequestDto.Action is null ||
            (portfolioTransactionRequestDto.Action != BuyStock && portfolioTransactionRequestDto.Action != SellStock))
        {
            throw new ValidationException(InvalidTransactionRequestAction);
        }

        if (portfolioTransactionRequestDto.Symbol is null)
        {
            throw new ValidationException(InvalidTransactionRequestSymbol);
        }
    }
}