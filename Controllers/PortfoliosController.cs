using Microsoft.AspNetCore.Mvc;
using StockTradingApplication.DTOs;
using StockTradingApplication.Services.Interfaces;

namespace StockTradingApplication.Controllers;

[ApiController]
[Route("api/v1/portfolios")]
public class PortfoliosController(IPortfolioService portfolioService) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult> CreatePortfolio([FromBody] CreatePortfolioRequestDto portfolioRequest)
    {
        var createdPortfolio = await portfolioService.CreatePortfolioAsync(portfolioRequest);
        return Created($"api/portfolios/{createdPortfolio.Id}", createdPortfolio);
    }
    
    [HttpDelete("{portfolioId:long}")]
    public async Task<IActionResult> DeletePortfolio(long portfolioId)
    {
        await portfolioService.DeletePortfolioAsync(portfolioId);
        return NoContent();
    }
}