using Microsoft.AspNetCore.Mvc;
using OrderManagement.Api.Authentication;

namespace OrderManagement.Api.Controllers;

[ApiController]
[Route("auth")]
public sealed class AuthController : ControllerBase
{
    private readonly TokenService _tokenService;

    public AuthController(TokenService tokenService)
    {
        _tokenService = tokenService;
    }

    [HttpPost("login")]
    public IActionResult Login(LoginRequest request)
    {
        const string validEmail = "dev@martech.com";
        const string validPassword = "Senha@123";

        if (request.Email != validEmail ||
            request.Password != validPassword)
        {
            return Unauthorized();
        }

        string token =
            _tokenService.GenerateToken(request.Email);

        LoginResponse response =
            new LoginResponse(token);

        return Ok(response);
    }
}

public sealed record LoginRequest(
    string Email,
    string Password);

public sealed record LoginResponse(
    string AccessToken);