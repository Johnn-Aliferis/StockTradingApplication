using System.Net;
using Microsoft.Extensions.Options;
using StockTradingApplication.Options;
using StockTradingApplication.DTOs;
using StockTradingApplication.Exceptions;
using StockTradingApplication.Services.Interfaces;

namespace StockTradingApplication.Services.Implementations;

public class ExternalStockService(HttpClient httpClient, IOptions<StockClientOptions> options) : IExternalStockService
{
    private readonly string _baseUrl = options.Value.BaseUrl;

    public async Task<IEnumerable<StockDataDto>> GetStockData()
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

            var stockDataDictionary = await response.Content.ReadFromJsonAsync<Dictionary<string, StockDataDto>>();
            return stockDataDictionary is null ? [] : [.. stockDataDictionary.Values];
        }

        catch (StockClientException ex)
        {
            throw new StockClientException("An error occurred while making the HTTP request.",
                ex.Status);
        }
    }
}