using Quartz;
using StockTradingApplication.Services;

namespace StockTradingApplication.Quartz.Jobs;

public class FetchStockDataJob(IStockService stockService) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        var stockData = await stockService.GetStockData();
        
        foreach (var stock in stockData)
        {
            Console.WriteLine(stock.Symbol);
        }
        
        await Task.CompletedTask;
    }
}