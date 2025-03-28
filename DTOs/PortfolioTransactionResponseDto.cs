namespace StockTradingApplication.DTOs;

public class PortfolioTransactionResponseDto
{
    public required long Id { get; set; }
    public required decimal StockPriceAtTransaction { get; set; }
    public required string TransactionType { get; set; }
    public required long StockId { get; set; }
    public required long PortfolioId { get; set; }
    public required decimal Quantity { get; set; }
}