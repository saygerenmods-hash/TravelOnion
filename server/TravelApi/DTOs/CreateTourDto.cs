
namespace TravelApi.DTOs;

public record CreateTourDto(
    string Name,
    DateTime StartDate,
    DateTime EndDate,
    decimal Price,
    int AvailablePlaces,
    int CountryId
);
