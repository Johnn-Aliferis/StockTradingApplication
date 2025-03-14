using StockTradingApplication.Entities;
using StockTradingApplication.Exceptions;

namespace StockTradingApplication.Services;

public class ValidationService
{
    private const string UserExists = "User already exists";
    private const string PortfolioExists = "A Portfolio for this user already exists";
    private const string UserDoesNotExist = "The provided id does not match to a registered user";

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
}