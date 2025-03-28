namespace StockTradingApplication.DTOs;

public class PortfolioTransactionRequestDto
{
    public required string Symbol { get; set; }
    public required decimal Quantity { get; set; }
}