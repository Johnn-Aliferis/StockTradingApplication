using Quartz;

namespace StockTradingApplication.Quartz.Jobs;

public class FetchStockDataJob : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        Console.WriteLine($"Hello, World! - {DateTime.Now}");
        await Task.CompletedTask;
    }
}