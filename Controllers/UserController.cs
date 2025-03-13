using Microsoft.AspNetCore.Mvc;
using StockTradingApplication.DTOs;
using StockTradingApplication.Services.Interfaces;

namespace StockTradingApplication.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController(IUserService userService) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult> CreateUser([FromBody] CreateUserRequestDto createUserRequestDto)
    {
        var createdUser = await userService.CreateUserAsync(createUserRequestDto);
        return Created($"/api/users/{createdUser.Id}", createdUser);
    }
}