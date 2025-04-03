using GridCityServer.Dtos;
using GridCityServer.Infrastructure;
using GridCityServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

namespace GridCityServer.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] AuthDto request)
    {
        var result = await _authService.LoginAsync(request.Username, request.Password);
        if (!result.IsSuccess)
            return Unauthorized(result);

        return Ok(result);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] AuthDto request)
    {
        var result = await _authService.RegisterAsync(request.Username, request.Password);
        if (!result.IsSuccess)
            return Unauthorized(result);

        return Ok(result);
    }
}
