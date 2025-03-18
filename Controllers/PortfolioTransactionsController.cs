using Microsoft.AspNetCore.Mvc;

namespace StockTradingApplication.Controllers;

[ApiController]
[Route("api/portfolios/{portfolioId:long}/transactions")]
public class PortfolioTransactionsController : ControllerBase
{
    // Todo : Boiler plate code here. Add exposed API for buying and selling stock. Since no JWT , add portfolio ID in request body.
    //      Also add another 2 entities --> Portfolio Balance for centralized balance , and Transaction_type for type of transactioN
    //      Will Include : BUY , SELL , INITIALIZE and apply EF core migrations for adding in DB during start up if not exists.

    [HttpPost("buy")]
    public async Task<IActionResult> BuyStock(long portfolioId)
    {
        return Ok();
    }
    
    [HttpPost("sell")]
    public async Task<IActionResult> SellStock(long portfolioId)
    {
        return Ok();
    }
}