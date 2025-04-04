﻿using System.Net;
using StockTradingApplication.DTOs;
using StockTradingApplication.Entities;
using StockTradingApplication.Exceptions;

namespace StockTradingApplication.Services;

public class ValidationService
{
    private const string UserExists = "User already exists";
    private const string PortfolioExists = "A Portfolio for this user already exists";
    private const string PortfolioNotExists = "The id provided does not correspond to a Portfolio";
    private const string UserDoesNotExist = "The provided user id does not match to a registered user";
    private const string InvalidTransactionRequest = "Invalid Transaction Request";
    private const string InvalidTransactionRequestQuantity = "Stock Quantity is invalid";
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
    
    public static void ValidateUpdateCashBalance(Portfolio portfolio)
    {
        if (portfolio is null)
        {
            throw new ValidationException(PortfolioNotExists);
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

        if (portfolioTransactionRequestDto.Symbol is null)
        {
            throw new ValidationException(InvalidTransactionRequestSymbol);
        }
    }
}