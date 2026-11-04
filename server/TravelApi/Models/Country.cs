
namespace TravelApi.Models;

public class Country
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    // Связь: одна страна — много туров
    public List<Tour> Tours { get; set; } = new();
}
