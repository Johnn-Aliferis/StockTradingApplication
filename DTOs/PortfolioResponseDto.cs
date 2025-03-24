namespace StockTradingApplication.DTOs;

public class PortfolioResponseDto
{
    public required long Id { get; set; }
    public required long UserId { get; set; }
    public required decimal CashBalance { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required DateTime UpdatedAt { get; set; }
}