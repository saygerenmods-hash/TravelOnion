
namespace TravelApi.Models;

public class Tour
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public decimal Price { get; set; }

    public int AvailablePlaces { get; set; }

    // Внешний ключ страны
    public int CountryId { get; set; }

    public Country? Country { get; set; }

    // Фотография тура
    public string? ImagePath { get; set; }

    // ID автора (для будущей авторизации)
    public int AuthorId { get; set; }
}
