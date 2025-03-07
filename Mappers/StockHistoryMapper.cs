using StockTradingApplication.DTOs;
using StockTradingApplication.Entities;

namespace StockTradingApplication.Mappers;

public class StockHistoryMapper
{
    public static StockHistoryDto ToStockHistoryDto(StockHistory stockHistory)
    {
        return new StockHistoryDto
        {
            Price = stockHistory.Price,
            CreatedAt = stockHistory.CreatedAt
        };
    }
}