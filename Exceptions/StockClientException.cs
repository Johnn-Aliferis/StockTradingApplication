using System.Net;

namespace StockTradingApplication.Exceptions;

public class StockClientException(string message, HttpStatusCode status) : GeneralException(message, status)
{
}