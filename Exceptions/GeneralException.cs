using System.Net;

namespace StockTradingApplication.Exceptions;

public class GeneralException : Exception
{
    public HttpStatusCode Status { get; }
    protected GeneralException(string message, HttpStatusCode status) : base(message) {
        Status = status;
    }
}