using System.Net;

namespace StockTradingApplication.Exceptions;

public class PortfolioTransactionException(string message, HttpStatusCode status) : GeneralException(message, status)
{
}