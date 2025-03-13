using AutoMapper;
using StockTradingApplication.DTOs;
using StockTradingApplication.Repository.Interfaces;
using StockTradingApplication.Services.Interfaces;

namespace StockTradingApplication.Services.Implementations;

public class StockHistoryService(IStockHistoryRepository stockHistoryRepository, IMapper mapper) : IStockHistoryService
{
    public async Task<List<StockHistoryDto>> GetStockHistoryAsync(string symbol)
    {
        var stockHistory = await stockHistoryRepository.GetStockHistory(symbol);
        return mapper.Map<List<StockHistoryDto>>(stockHistory);
    }
}