using Quartz;
using StockTradingApplication.Mappers;
using StockTradingApplication.Services;

namespace StockTradingApplication.Quartz.Jobs;

public class FetchStockDataJob(IExternalStockService externalStockService, IStockDbService stockDbService) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        var stockData = await externalStockService.GetStockData();
        
        foreach (var stockDataDto in stockData)
        {
            // var stock = StockMapper.ToStockEntity(stockDataDto);
            // await stockDbService.SaveOrUpdateStockAsync(stock);
            // todo : continue next time and call the appropriate methods here.
        }
        
        await Task.CompletedTask;
    }
}