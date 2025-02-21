namespace StockTradingApplication.DTOs;

public class SqlQueryDto(string query, object[] parameters)
{
    public string Query { get; } = query;
    public object[] Parameters { get; } = parameters ; 
}
