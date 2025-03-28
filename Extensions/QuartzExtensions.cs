using Quartz;
using StockTradingApplication.Quartz.Jobs;

namespace StockTradingApplication.Extensions;

public static class QuartzExtensions
{
    public static IServiceCollection AddQuartzJobs(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddQuartz(quartz =>
        {
            var jobKey = new JobKey("FetchStockDataJob");
            quartz.AddJob<FetchStockDataJob>(opts => opts.WithIdentity(jobKey));
            quartz.AddTrigger(opts =>
                opts.ForJob(jobKey)
                    .WithIdentity("FetchStockDataJobTrigger")
                    .WithSimpleSchedule(schedule =>
                        schedule.WithIntervalInSeconds(60)
                            .RepeatForever()));
        });

        // For graceful shutdown
        serviceCollection.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);

        return serviceCollection;
    }
}