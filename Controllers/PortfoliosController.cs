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
    
    [HttpGet]
    [Route("{portfolioId:long}")]
    public async Task<ActionResult> GetPortfolioById(long portfolioId)
    {
        var portfolio = await portfolioService.GetPortfolioAsync(portfolioId);
        return Ok(portfolio);
    }
    
    [HttpGet]
    [Route("{portfolioId:long}/holdings")]
    public async Task<ActionResult> GetPortfolioHolding(long portfolioId)
    {
        var holdings = await portfolioService.GetPortfolioHoldingsByPortfolioIdAsync(portfolioId);
        return Ok(holdings);
    }
}