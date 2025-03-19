namespace StockTradingApplication.DTOs;

public class PortfolioBalanceResponseDto
{
    public required long Id { get; set; }
    public required decimal Balance { get; set; }
    public required long PortfolioId { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required DateTime UpdatedAt { get; set; }
}