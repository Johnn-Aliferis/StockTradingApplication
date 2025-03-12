using Microsoft.AspNetCore.Mvc;

namespace StockTradingApplication.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransactionsController : ControllerBase
{
    // Todo : Boiler plate code here. Add exposed API for buying and selling stock. Since no JWT , add portfolio ID in request body.
    //      Also add another 2 entities --> Portfolio Balance for centralized balance , and Transaction_type for type of transactioN
    //      Will Include : BUY , SELL , INITIALIZE and apply EF core migrations for adding in DB during start up if not exists.
}