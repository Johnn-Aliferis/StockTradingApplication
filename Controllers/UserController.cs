using Microsoft.AspNetCore.Mvc;
using StockTradingApplication.DTOs;
using StockTradingApplication.Services;

namespace StockTradingApplication.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController(IUserService userService) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult> CreateUser([FromBody] CreateUserDto createUserDto)
    {
        var createdUser = await userService.CreateUserAsync(createUserDto);

        return Created($"/api/users/{createdUser.Id}", createdUser);

    }
}