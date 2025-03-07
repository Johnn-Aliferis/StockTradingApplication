namespace StockTradingApplication.DTOs;

public class StockHistoryDto
{
    public required decimal Price { get; set; }
    public required DateTime CreatedAt { get; set; }
}