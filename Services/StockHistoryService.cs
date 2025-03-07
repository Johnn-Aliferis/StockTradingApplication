using StockTradingApplication.DTOs;
using StockTradingApplication.Mappers;
using StockTradingApplication.Repository;

namespace StockTradingApplication.Services;

public class StockHistoryService(IStockHistoryRepository stockHistoryRepository) : IStockHistoryService
{
    public async Task<List<StockHistoryDto>> GetStockHistoryAsync(string symbol)
    {
        var stockHistory = await stockHistoryRepository.GetStockHistory(symbol);
        return stockHistory.Select(StockHistoryMapper.ToStockHistoryDto).ToList();
    }
}