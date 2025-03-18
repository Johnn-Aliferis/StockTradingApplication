namespace StockTradingApplication.DTOs;

public class PortfolioTransactionRequestDto
{
    public required string Action { get; set; }
    public required string Symbol { get; set; }
    public required decimal Quantity { get; set; }
}