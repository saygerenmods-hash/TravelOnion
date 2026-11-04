
namespace TravelApi.DTOs;

public record LoginResponseDto(
    string Token,
    string TokenType,
    DateTime ExpiresAt,
    UserDto User
);
