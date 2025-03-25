using Microsoft.AspNetCore.Mvc;
using StockTradingApplication.DTOs;
using StockTradingApplication.Services.Interfaces;

namespace StockTradingApplication.Controllers;

[ApiController]
[Route("api/v1/portfolios")]
public class PortfoliosController(IPortfolioService portfolioService) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult> CreatePortfolio([FromBody] PortfolioRequestDto portfolioRequest)
    {
        var createdPortfolio = await portfolioService.CreatePortfolioAsync(portfolioRequest);
        return Created($"api/portfolios/{createdPortfolio.Id}", createdPortfolio);
    }
    
    [HttpPost]
    [Route("{portfolioId:long}/balance")] // for simplicity , only adding funds.
    public async Task<ActionResult> AddPortfolioBalance([FromBody] PortfolioRequestDto portfolioRequest, long portfolioId)
    {
        var createdPortfolio = await portfolioService.AddPortfolioBalance(portfolioRequest, portfolioId);
        return Ok(createdPortfolio);
    }
    
    [HttpDelete("{portfolioId:long}")]
    public async Task<IActionResult> DeletePortfolio(long portfolioId)
    {
        await portfolioService.DeletePortfolioAsync(portfolioId);
        return NoContent();
    }
}