using System.Net;
using Microsoft.Extensions.Options;
using StockTradingApplication.Configuration;
using StockTradingApplication.DTOs;
using StockTradingApplication.Exceptions;

namespace StockTradingApplication.Services;

public class StockService(HttpClient httpClient, IOptions<StockClientOptions> options) : IStockService
{
    private readonly string _baseUrl = options.Value.BaseUrl;

    public async Task<IEnumerable<StockData>> GetStockData()
    {
        var apiKey = Environment.GetEnvironmentVariable("API_KEY");

        if (string.IsNullOrWhiteSpace(apiKey))
        {
            throw new StockClientException("API key not found", HttpStatusCode.BadRequest);
        }

        try
        {
            var response = await httpClient.GetAsync(_baseUrl + apiKey);

            if (!response.IsSuccessStatusCode)
            {
                throw new StockClientException("Error fetching stock data", response.StatusCode);
            }

            var stockData = await response.Content.ReadFromJsonAsync<IEnumerable<StockData>>();

            return stockData ?? new List<StockData>();
        }
        
        catch (Exception ex)
        {
            throw new StockClientException("An error occurred while making the HTTP request.", HttpStatusCode.InternalServerError);
        }
    }
}

// 1) Code first with entity framework.
// 2) Stock Data Fetcher service (classic interface architecture , later pass on mediators) -- Periodic Job
// 3) For the above , ask ai if for example we should / can use other design patterns , or just plain service-repository pattern.