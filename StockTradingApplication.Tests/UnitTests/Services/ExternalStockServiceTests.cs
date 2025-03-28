using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using StockTradingApplication.DTOs;
using StockTradingApplication.Exceptions;
using StockTradingApplication.Options;
using StockTradingApplication.Services.Implementations;
using Xunit.Abstractions;

namespace StockTradingApplication.Tests.UnitTests.Services;

public class ExternalStockServiceTests
{
    private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
    private readonly HttpClient _httpClient;
    private readonly ExternalStockService _externalStockService;
    private readonly Mock<IOptions<StockClientOptions>> _optionsMock;
    private readonly ITestOutputHelper _output;

    public ExternalStockServiceTests(ITestOutputHelper output)
    {
        _output = output;
        _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
        _httpClient = new HttpClient(_httpMessageHandlerMock.Object);

        _optionsMock = new Mock<IOptions<StockClientOptions>>();
        _optionsMock.Setup(opt => opt.Value)
            .Returns(new StockClientOptions
                { BaseUrl = "https://api.twelvedata.com/quote?symbol=AAPL,MSFT,GOOGL,AMZN,META&apikey=" });

        _externalStockService = new ExternalStockService(_httpClient, _optionsMock.Object);
    }

    [Fact]
    public async Task Test_API_key_missing()
    {
        Environment.SetEnvironmentVariable("API_KEY", null);

        var exception = await Assert.ThrowsAsync<StockClientException>(() => _externalStockService.GetStockData());
        Assert.Equal("API key not found", exception.Message);
        Assert.Equal(HttpStatusCode.BadRequest, exception.Status);
        
        _output.WriteLine("Test_API_key_missing PASSED SUCCESSFULLY!");
    }

    [Fact]
    public async Task GetStockData_HttpRequest_Fail()
    {
        Environment.SetEnvironmentVariable("API_KEY", "test_api_key");

        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.InternalServerError
            });

        var exception = await Assert.ThrowsAsync<StockClientException>(() => _externalStockService.GetStockData());
        Assert.Equal("An error occurred while making the HTTP request.", exception.Message);
        Assert.Equal(HttpStatusCode.InternalServerError, exception.Status);
        
        _output.WriteLine("GetStockData_HttpRequest_Fail PASSED SUCCESSFULLY!");
    }

    [Fact]
    public async Task GetStockData_Successful()
    {
        Environment.SetEnvironmentVariable("API_KEY", "valid_api_key");
        var stockData = new Dictionary<string, StockDataDto>
        {
            { "AAPL", new StockDataDto { Symbol = "AAPL", Name = "Test AAPL", Close = 15.00m, Currency = "USD" } },
            { "MSFT", new StockDataDto { Symbol = "MSFT", Name = "Test MSFT", Close = 25.00m, Currency = "USD" } }
        };

        var jsonStockData = JsonSerializer.Serialize(stockData);

        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(jsonStockData)
            });
        
        var result = await _externalStockService.GetStockData();
        var resultList = result.ToList();
        
        Assert.NotNull(result);
        Assert.Equal(2, resultList.Count);
        Assert.Contains(resultList, s => s.Symbol == "AAPL" && s.Close == 15.00m);
        Assert.Contains(resultList, s => s.Symbol == "MSFT" && s.Close == 25.00m);
        
        _output.WriteLine("GetStockData_Successful PASSED SUCCESSFULLY!");
    }
}