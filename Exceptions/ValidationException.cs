using System.Net;

namespace StockTradingApplication.Exceptions;

public class ValidationException(string message) : GeneralException(message, HttpStatusCode.BadRequest)
{
}