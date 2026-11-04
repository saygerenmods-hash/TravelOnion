
namespace TravelApi.Models;

public class User
{
    public int Id { get; set; }

    // Имя пользователя
    public string Username { get; set; } = string.Empty;

    // Электронная почта
    public string Email { get; set; } = string.Empty;

    // Хеш пароля (не сам пароль!)
    public string PasswordHash { get; set; } = string.Empty;

    // Дата регистрации
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Туры, созданные пользователем
    public List<Tour> Tours { get; set; } = new();
}
