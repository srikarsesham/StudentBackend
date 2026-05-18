using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using StudentApi.DTOs;
using StudentApi.Options;

namespace StudentApi.Controllers;

[AllowAnonymous]
[ApiController]
[Route("api/[controller]")]
public class AuthController(IOptions<JwtOptions> jwtOptions) : ControllerBase
{
    private readonly JwtOptions _jwtOptions = jwtOptions.Value;

    [HttpPost("login")]
    public IActionResult Login(LoginRequestDto dto)
    {
        if (!string.Equals(dto.Username, _jwtOptions.DemoUsername, StringComparison.Ordinal) ||
            !string.Equals(dto.Password, _jwtOptions.DemoPassword, StringComparison.Ordinal))
        {
            return Unauthorized(new
            {
                message = "Invalid username or password."
            });
        }

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, dto.Username),
            new Claim(ClaimTypes.Role, "Admin")
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiresAt = DateTime.UtcNow.AddHours(1);

        var token = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials);

        var serializedToken = new JwtSecurityTokenHandler().WriteToken(token);

        return Ok(new LoginResponseDto
        {
            Token = serializedToken,
            ExpiresAt = expiresAt
        });
    }
}
