using Quartz;
using StockTradingApplication.Services.Interfaces;

namespace StockTradingApplication.Quartz.Jobs;

public class FetchStockDataJob(IExternalStockService externalStockService, IStockDbService stockDbService) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        var stockData = await externalStockService.GetStockData();
        await stockDbService.HandleExternalProviderData(stockData.ToList());
        await Task.CompletedTask;
    }
}