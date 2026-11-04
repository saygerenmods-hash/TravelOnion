
namespace TravelApi.DTOs;

public record TourDto(
    int Id,
    string Name,
    DateTime StartDate,
    DateTime EndDate,
    decimal Price,
    int AvailablePlaces,
    int CountryId,
    string CountryName,
    string? ImagePath
);
