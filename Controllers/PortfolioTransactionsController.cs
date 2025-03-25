using Microsoft.AspNetCore.Mvc;
using StockTradingApplication.DTOs;
using StockTradingApplication.Services.Interfaces;

namespace StockTradingApplication.Controllers;

[ApiController]
[Route("api/v1/portfolios/{portfolioId:long}/transactions")]
public class PortfolioTransactionsController(IPortfolioTransactionService portfolioTransactionService) : ControllerBase
{
    [HttpPost("buy")]
    public async Task<IActionResult> BuyStock([FromBody] PortfolioTransactionRequestDto portfolioTransactionRequestDto,
        [FromRoute] long portfolioId)
    {
        var createdTransaction =
            await portfolioTransactionService.BuyStockAsync(portfolioTransactionRequestDto, portfolioId);

        return Created($"/api/transactions/{createdTransaction.Id}", createdTransaction);
    }

    [HttpPost("sell")]
    public async Task<IActionResult> SellStock([FromBody] PortfolioTransactionRequestDto portfolioTransactionRequestDto,
        [FromRoute] long portfolioId)
    {
        var createdTransaction =
            await portfolioTransactionService.SellStockAsync(portfolioTransactionRequestDto, portfolioId);

        return Created($"/api/transactions/{createdTransaction.Id}", createdTransaction);
    }
}