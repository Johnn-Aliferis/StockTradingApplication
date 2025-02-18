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

    public async Task SaveOrUpdateStockAsync(Stock stock)
    {
        await stockRepository.SaveOrUpdateStockAsync(stock);
        // need to also see what happens and best practise for the history of stocks :
        // what is best practise  ?  What if we have maaaany stocks , how do we address that ? we cant loop a thousand times ...!
    }
}