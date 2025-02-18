namespace StockTradingApplication.DTOs;

public class StockDataDto
{
    public required string Symbol { get; set; }
    public required string Name { get; set; }
    public required string Currency { get; set; }
    public required decimal Close { get; set; }
    public required DateTime Datetime { get; set; }
}