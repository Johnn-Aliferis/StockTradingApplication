using System.Net;

namespace StockTradingApplication.Exceptions;

public class PortfolioException(string message, HttpStatusCode status) : GeneralException(message, status)
{
}