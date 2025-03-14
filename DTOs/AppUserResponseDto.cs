namespace StockTradingApplication.DTOs;

public class AppUserResponseDto
{
    public required long Id { get; set; }
    public required string Username { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required DateTime UpdatedAt { get; set; }
}