using System.Text;
using StockTradingApplication.DTOs;
using StockTradingApplication.Entities;
using StockTradingApplication.Repository;

namespace StockTradingApplication.Services;

public class StockDbService(IStockRepository stockRepository) : IStockDbService

{
    public async Task<List<Stock>> GetStocksAsync()
    {
        return await stockRepository.GetStocksAsync();
    }

    public async Task<Stock?> GetStockAsync(string symbol)
    {
        return await stockRepository.GetStockAsync(symbol);
    }
    public async Task HandleExternalProviderData(List<StockDataDto> externalStockData)
    {
        var existingStocks = await GetStocksAsync();
        var existingStockSymbols = existingStocks.Select(s => s.Symbol).ToHashSet();
        var stockHistories = new List<StockHistory>();

        var stocksToInsert = externalStockData.Where(dto => !existingStockSymbols.Contains(dto.Symbol)).ToList();
        var stocksToUpdate = externalStockData.Where(dto => existingStockSymbols.Contains(dto.Symbol)).ToList();
        var dateNow = DateTime.UtcNow;

        // Insert new stocks
        var newStocks = stocksToInsert.Select(s => new Stock
        {
            Symbol = s.Symbol,
            Price = s.Close,
            Name = s.Name,
            Currency = s.Currency,
        }).ToList();
        
        // Update existing stocks and create history records
        foreach (var stock in existingStocks)
        {
            var updatedStock = stocksToUpdate.First(s => s.Symbol == stock.Symbol);
            stock.Symbol = updatedStock.Symbol;
            stock.Name = updatedStock.Name;
            stock.Price = updatedStock.Close;
            stock.UpdatedAt = dateNow;
            
            // Create history record
            var stockHistory = new StockHistory
            {
                Price = stock.Price,
                CreatedAt = dateNow,
                StockId = stock.Id
            };
            stockHistories.Add(stockHistory);
        }
        // Perform bulk operations
        await stockRepository.HandleInsertAndUpdateBulkOperationAsync(newStocks, existingStocks, stockHistories);
    }
}