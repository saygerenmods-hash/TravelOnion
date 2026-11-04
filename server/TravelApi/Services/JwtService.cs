
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using TravelApi.DTOs;
using TravelApi.Models;

namespace TravelApi.Services;

public class JwtService
{
    private readonly IConfiguration _configuration;

    public JwtService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public LoginResponseDto GenerateToken(User user)
    {
        // Получаем секретный ключ из User Secrets
        var secretKey = _configuration["Jwt:Key"]
            ?? throw new InvalidOperationException(
                "JWT key is not configured");

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(secretKey));

        // Подписываем токен
        var credentials = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256);

        // Данные пользователя внутри токена
        var claims = new[]
        {
            new Claim(
                ClaimTypes.NameIdentifier,
                user.Id.ToString()),

            new Claim(
                ClaimTypes.Name,
                user.Username),

            new Claim(
                ClaimTypes.Email,
                user.Email)
        };

        // Токен действителен 2 часа
        var expiresAt = DateTime.UtcNow.AddHours(2);

        var token = new JwtSecurityToken(
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials
        );

        var tokenString =
            new JwtSecurityTokenHandler().WriteToken(token);

        return new LoginResponseDto(
            tokenString,
            "Bearer",
            expiresAt,
            new UserDto(
                user.Id,
                user.Username,
                user.Email)
        );
    }
}
