using System.Text;
using StockTradingApplication.DTOs;
using StockTradingApplication.Entities;
using StockTradingApplication.Repository;

namespace StockTradingApplication.Services;

public class StockDbService(IStockRepository stockRepository) : IStockDbService

{
    
    public async Task<List<Stock>> GetStocksAsync()
    {
        return await stockRepository.GetStocksAsync();
    }

    public async Task<Stock?> GetStockAsync(string symbol)
    {
        return await stockRepository.GetStockAsync(symbol);
    }
    
    public async Task HandleExternalProviderData(List<StockDataDto> externalStockData)
    {
        var existingStockSymbols =  await stockRepository.GetSymbolsAsync();
        var existingStockSymbolsSet = existingStockSymbols.ToHashSet();
        
        var stocksToUpdateSymbols = externalStockData
            .Where(dto => existingStockSymbolsSet.Contains(dto.Symbol))
            .Select(dto => dto.Symbol)
            .ToList();

        var mergeSqlQueryDto = GenerateBulkMergeStockQuery(externalStockData);
        var historySqlQueryDto = GenerateBulkInsertStockHistoryQuery(stocksToUpdateSymbols);
        
        // Perform bulk operations
        await stockRepository.HandleInsertAndUpdateBulkOperationAsync(mergeSqlQueryDto , historySqlQueryDto);
    }
    
    private static SqlQueryDto GenerateBulkMergeStockQuery(List<StockDataDto> stockList)
    {
        if (stockList.Count == 0)
        {
            return new SqlQueryDto(string.Empty, []);
        }

        var sql = new StringBuilder("MERGE INTO stock AS target USING (VALUES ");
        var parameters = new List<object>();
        var values = new List<string>();
        var dateNow = DateTime.UtcNow;

        var index = 0;
        foreach (var stock in stockList)
        {
            values.Add($"(@p{index}, @p{index + 1}, @p{index + 2}, @p{index + 3}, @p{index + 4} , @p{index + 5})");

            parameters.Add(stock.Symbol);     
            parameters.Add(stock.Name);       
            parameters.Add(stock.Close);      
            parameters.Add(stock.Currency);   
            parameters.Add(dateNow);  
            parameters.Add(dateNow);  

            index += 6;
        }

        sql.Append(string.Join(",", values));
        sql.Append(") AS source (symbol, name, price, currency, created_at, updated_at) ");
        sql.Append("ON target.symbol = source.symbol ");

        sql.Append("WHEN MATCHED THEN UPDATE SET ");
        sql.Append("stock_name = source.name, ");
        sql.Append("stock_price = source.price, ");
        sql.Append("stock_currency = source.currency, ");
        sql.Append("updated_at = source.updated_at ");

        sql.Append("WHEN NOT MATCHED THEN INSERT (stock_symbol, stock_name, stock_price, stock_currency, created_at, updated_at) ");
        sql.Append("VALUES (source.symbol, source.name, source.price, source.currency, source.created_at, source.updated_at);");

        return new SqlQueryDto(sql.ToString(), parameters.ToArray());
    }
    
    
    
    private static SqlQueryDto GenerateBulkInsertStockHistoryQuery(List<string> existingSymbols)
    {
        if (existingSymbols.Count == 0)
        {
            return new SqlQueryDto(string.Empty, []);
        }
        
        var sql = new StringBuilder("INSERT INTO stock_history (stock_id, price, created_at) SELECT id, price, @p0 FROM stock WHERE symbol IN (");
        var parameters = new List<object> { DateTime.UtcNow };
        
        var index = 1;
        var symbolParams = new List<string>();

        foreach (var symbol in existingSymbols)
        {
            var paramName = $"@p{index}";
            symbolParams.Add(paramName);
            parameters.Add(symbol);
            index++;
        }

        sql.Append(string.Join(",", symbolParams));
        sql.Append(");");

        return new SqlQueryDto(sql.ToString(), parameters.ToArray());
    }
}