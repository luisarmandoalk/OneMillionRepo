using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using OneMillionCopy.Leads.Api.Auth;
using OneMillionCopy.Leads.Api.Contracts.Auth;

namespace OneMillionCopy.Leads.Api.Controllers;

[ApiController]
[Route("auth")]
public sealed class AuthController : ControllerBase
{
    private readonly AuthOptions _authOptions;
    private readonly JwtOptions _jwtOptions;
    private readonly JwtTokenGenerator _jwtTokenGenerator;

    public AuthController(
        IOptions<AuthOptions> authOptions,
        IOptions<JwtOptions> jwtOptions,
        JwtTokenGenerator jwtTokenGenerator)
    {
        _authOptions = authOptions.Value;
        _jwtOptions = jwtOptions.Value;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        if (!string.Equals(request.Username, _authOptions.Username, StringComparison.Ordinal) ||
            !string.Equals(request.Password, _authOptions.Password, StringComparison.Ordinal))
        {
            return Unauthorized(new
            {
                error = "Credenciales invalidas."
            });
        }

        var token = _jwtTokenGenerator.GenerateToken(request.Username);

        return Ok(new LoginResponse(
            token,
            "Bearer",
            _jwtOptions.ExpirationMinutes * 60));
    }
}
