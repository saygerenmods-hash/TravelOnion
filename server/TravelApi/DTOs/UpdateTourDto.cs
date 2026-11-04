
namespace TravelApi.DTOs;

public record UpdateTourDto(
    string Name,
    DateTime StartDate,
    DateTime EndDate,
    decimal Price,
    int AvailablePlaces,
    int CountryId
);
