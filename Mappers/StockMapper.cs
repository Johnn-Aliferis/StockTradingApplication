using StockTradingApplication.DTOs;
using StockTradingApplication.Entities;

namespace StockTradingApplication.Mappers;

public static class StockMapper
{
    public static Stock ToStockEntity(StockDataDto stockDataDto)
    {
        return new Stock
        {
            Symbol = stockDataDto.Symbol,
            Name = stockDataDto.Name,
            Price = stockDataDto.Close,
            Currency = stockDataDto.Currency,
        };
    }
}