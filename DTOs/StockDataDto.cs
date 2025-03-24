namespace StockTradingApplication.DTOs;

public class StockDataDto
{
    public long Id { get; set; }
    public required string Symbol { get; set; }
    public required string Name { get; set; }
    public required string Currency { get; set; }
    public required decimal Close { get; set; }
}