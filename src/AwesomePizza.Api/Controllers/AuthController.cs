using AwesomePizza.Service.Dtos;
using AwesomePizza.Service.Services;
using Microsoft.AspNetCore.Mvc;

namespace AwesomePizza.Api.Controllers;

/// <summary>Login del personale di cucina.</summary>
[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>Restituisce un token JWT da usare negli endpoint della cucina.</summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        return Ok(await _authService.LoginAsync(request));
    }
}