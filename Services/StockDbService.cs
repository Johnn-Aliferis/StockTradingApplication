using System.Text;
using AutoMapper;
using StockTradingApplication.DTOs;
using StockTradingApplication.Mappers;
using StockTradingApplication.Repository;

namespace StockTradingApplication.Services;

public class StockDbService(IStockRepository stockRepository, IMapper mapper) : IStockDbService

{
    
    public async Task<List<StockDataDto>> GetStocksAsync()
    {
        var stocks = await stockRepository.GetStocksAsync();
        return mapper.Map<List<StockDataDto>>(stocks);
    }

    public async Task<StockDataDto?> GetStockAsync(string symbol)
    {
        var stock = await stockRepository.GetStockAsync(symbol);
        return stock is not null ? mapper.Map<StockDataDto>(stock) : null;
    }
    
    public async Task<List<StockDataDto>> HandleExternalProviderData(List<StockDataDto> externalStockData)
    {
        var dateNow = DateTime.UtcNow;
        
        var mergeSqlQueryDto = GenerateBulkMergeStockQuery(externalStockData, dateNow);
        var historySqlQueryDto = GenerateBulkInsertStockHistoryQuery(externalStockData, dateNow);
        
        // Perform bulk operations
        var result = await stockRepository.HandleInsertAndUpdateBulkOperationAsync(mergeSqlQueryDto , historySqlQueryDto);

        return mapper.Map<List<StockDataDto>>(result);
    }
    
    private static SqlQueryDto GenerateBulkMergeStockQuery(List<StockDataDto> stockList, DateTime dateNow)
    {
        if (stockList.Count == 0)
        {
            return new SqlQueryDto(string.Empty, []);
        }

        var sql = new StringBuilder("MERGE INTO stock AS target USING (VALUES ");
        var parameters = new List<object>();
        var values = new List<string>();

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
        sql.Append("ON target.stock_symbol = source.symbol ");
        
        sql.Append("WHEN MATCHED AND (");
        sql.Append("target.stock_price IS DISTINCT FROM source.price "); 
        
        sql.Append(") THEN UPDATE SET "); 
        sql.Append("stock_price = source.price, ");
        sql.Append("updated_at = source.updated_at ");

        sql.Append("WHEN NOT MATCHED THEN INSERT (stock_symbol, stock_name, stock_price, stock_currency, created_at, updated_at) ");
        sql.Append("VALUES (source.symbol, source.name, source.price, source.currency, source.created_at, source.updated_at) ");
        
        sql.Append("RETURNING target.id, target.stock_symbol, target.stock_name, target.stock_price, target.stock_currency, target.created_at , target.updated_at;");
        
        return new SqlQueryDto(sql.ToString(), parameters.ToArray());
    }
    
    private static SqlQueryDto GenerateBulkInsertStockHistoryQuery(List<StockDataDto> stockList, DateTime dateNow)
    {
        if (stockList.Count == 0)
        {
            return new SqlQueryDto(string.Empty, []);
        }
        
        var sql = new StringBuilder();
        var parameters = new List<object>();
        var values = new List<string>();
        
        sql.Append("WITH UpdatedRows AS ( ");
        sql.Append("SELECT target.id, target.stock_symbol, target.stock_name, target.stock_price, target.stock_currency ");
        sql.Append("FROM stock AS target ");
        sql.Append("INNER JOIN (VALUES ");
        
        var index = 0;
        foreach (var stock in stockList)
        {
            values.Add($"(@p{index}, @p{index + 1}, @p{index + 2}, @p{index + 3})");

            parameters.Add(stock.Symbol);     
            parameters.Add(stock.Name);       
            parameters.Add(stock.Close);      
            parameters.Add(stock.Currency);   
            index += 4;
        }

        sql.Append(string.Join(",", values));
        sql.Append(") ");
        sql.Append("AS source (symbol, name, price, currency) ");
        sql.Append("ON target.stock_symbol = source.symbol ");
        sql.Append("WHERE target.stock_price <> source.price ");
        sql.Append(") ");
        
        sql.Append("INSERT INTO stock_history (stock_price, created_at,updated_at, stock_id) ");
        sql.Append("SELECT stock_price, @p" + index + ", @p" + index + ", id FROM UpdatedRows;");
        parameters.Add(dateNow);


        return new SqlQueryDto(sql.ToString(), parameters.ToArray());
    }
}