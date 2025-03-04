using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using StockTradingApplication.DTOs;
using StockTradingApplication.Services;

namespace StockTradingApplication.Decorators;

public class StockDbServiceCachingDecorator(
    IStockDbService decoratedService,
    IDistributedCache cache,
    ILogger<StockDbServiceCachingDecorator> logger) : IStockDbService
{
    public async Task<List<StockDataDto>> GetStocksAsync()
    {
        const string cacheKey = "stocks";
        var cachedStocks = await cache.GetStringAsync(cacheKey);

        if (!string.IsNullOrEmpty(cachedStocks))
        {
            logger.LogInformation("Fetching stocks from Redis");
            return JsonSerializer.Deserialize<List<StockDataDto>>(cachedStocks)!;
        }

        logger.LogInformation("Fetching stocks from database");
        var stocks = await decoratedService.GetStocksAsync();
        var cacheValue = JsonSerializer.Serialize(stocks);

        logger.LogInformation("Persisting stocks into Redis");
        await cache.SetStringAsync(cacheKey, cacheValue, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = null
        });

        return stocks;
    }

    public async Task<StockDataDto?> GetStockAsync(string symbol)
    {
        var cacheKey = $"{symbol}";
        var cachedStock = await cache.GetStringAsync(cacheKey);

        if (!string.IsNullOrEmpty(cachedStock))
        {
            logger.LogInformation("Fetching stock from Redis");
            return JsonSerializer.Deserialize<StockDataDto>(cachedStock);
        }
        
        logger.LogInformation("Fetching stocks from database");
        var stock = await decoratedService.GetStockAsync(symbol);
        var cacheValue = JsonSerializer.Serialize(stock);
        
        logger.LogInformation("Persisting stocks into Redis");
        await cache.SetStringAsync(cacheKey, cacheValue, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = null
        });
        
        return stock;
    }

    public async Task<List<StockDataDto>> HandleExternalProviderData(List<StockDataDto> stockDataDtos)
    {
        var updatedStocks = await decoratedService.HandleExternalProviderData(stockDataDtos);
        
        if (updatedStocks.Count == 0)
        {
            logger.LogInformation("No changes detected. Skipping cache update.");
            return updatedStocks;
        }

        var batch = updatedStocks
            .Select(
                stock => cache.SetStringAsync(
                $"{stock.Symbol}",
                JsonSerializer.Serialize(stock),
                new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = null }
            ))
            .ToList();
        
        await Task.WhenAll(batch);

        await UpdateFullStockCacheAsync(stockDataDtos);
        
        return updatedStocks;
    }

    private async Task UpdateFullStockCacheAsync(List<StockDataDto> stockDataDtos)
    {
        const string cacheKey = "stocks";
        var cacheValue = JsonSerializer.Serialize(stockDataDtos);
        
        await cache.SetStringAsync(cacheKey, cacheValue, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = null
        });
    }
}