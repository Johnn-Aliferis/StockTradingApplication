using Microsoft.AspNetCore.Mvc;
using StockTradingApplication.DTOs;

namespace StockTradingApplication.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PortfoliosController : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult> CreatePortfolio([FromBody] CreatePortfolioRequestDto portfolioRequest)
    {
        return Ok();
    }
}