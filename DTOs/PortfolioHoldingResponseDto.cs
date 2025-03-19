namespace StockTradingApplication.DTOs;

public class PortfolioHoldingResponseDto
{
    public required long Id { get; set; }
    public required decimal StockQuantity { get; set; }
    public required long PortfolioId { get; set; }
    public required long StockId { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required DateTime UpdatedAt { get; set; }
}