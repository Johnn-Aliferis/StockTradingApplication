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

            var stockDataDictionary = await response.Content.ReadFromJsonAsync<Dictionary<string, StockData>>();
            return stockDataDictionary is null ? [] : [.. stockDataDictionary.Values];
        }

        catch (Exception ex)
        {
            throw new StockClientException("An error occurred while making the HTTP request.",
                HttpStatusCode.InternalServerError);
        }
    }
}