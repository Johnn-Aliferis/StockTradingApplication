using AutoMapper;
using Moq;
using StockTradingApplication.DTOs;
using StockTradingApplication.Entities;
using StockTradingApplication.Exceptions;
using StockTradingApplication.Repository.Interfaces;
using StockTradingApplication.Services.Implementations;
using Xunit.Abstractions;

namespace StockTradingApplication.Tests.UnitTests.Services;

public class UserServiceTests
{
    private readonly UserService _userService;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly ITestOutputHelper _output;

    public UserServiceTests(ITestOutputHelper output)
    {
        _output = output;
        _userRepositoryMock = new Mock<IUserRepository>();
        _mapperMock = new Mock<IMapper>();
        _userService = new UserService(_userRepositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task CreateUserAsync_Success()
    {
        var date = DateTime.UtcNow;
        
        var requestDto = new CreateUserRequestDto { Username = "testuser" };
        var responseDto = new AppUserResponseDto { Id = 1, Username = "testuser", CreatedAt = date, UpdatedAt = date };
        
        var newUser = new AppUser { Id = 1, Username = "testuser", CreatedAt = date, UpdatedAt = date };
        AppUser? existingUser = null;

        _userRepositoryMock.Setup(repo => repo.GetUserAsync(requestDto.Username)).ReturnsAsync(existingUser);

        _mapperMock.Setup(mapper => mapper.Map<AppUser>(requestDto)).Returns(newUser);

        _userRepositoryMock.Setup(repo => repo.SaveUserAsync(newUser)).ReturnsAsync(newUser);

        _mapperMock.Setup(mapper => mapper.Map<AppUserResponseDto>(newUser)).Returns(responseDto);
        
        var result = await _userService.CreateUserAsync(requestDto);
        
        Assert.NotNull(result);
        Assert.Equal(responseDto.Id, result.Id);
        Assert.Equal(responseDto.Username, result.Username);
        _userRepositoryMock.Verify(repo => repo.SaveUserAsync(newUser), Times.Once);
        
        _output.WriteLine(" CreateUserAsync_Success PASSED SUCCESSFULLY!");
    }
    
    [Fact]
    public async Task CreateUserAsync_Failure()
    {
        var date = DateTime.UtcNow;
        
        var requestDto = new CreateUserRequestDto { Username = "testuser" };
        var existingUser = new AppUser() {Username = "testuser", CreatedAt = date , UpdatedAt = date };

        _userRepositoryMock.Setup(repo => repo.GetUserAsync(requestDto.Username)).ReturnsAsync(existingUser);

        var exception = await Assert.ThrowsAsync<ValidationException>(async () => 
            await _userService.CreateUserAsync(requestDto)
        );
        
        Assert.Equal("User already exists", exception.Message);
        
        _output.WriteLine(" CreateUserAsync_Failure PASSED SUCCESSFULLY!");
    }
}