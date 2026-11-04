
using Microsoft.EntityFrameworkCore;
using TravelApi.Data;
using TravelApi.DTOs;

namespace TravelApi.Services;

public static class TourEndpoints
{
    public static RouteGroupBuilder MapTourEndpoints(
        this WebApplication app)
    {
        var group = app.MapGroup("/api/tours")
            .WithTags("Tours");

        // GET: Получить список всех туров
        group.MapGet("/", async (AppDbContext db) =>
        {
            var tours = await db.Tours
                .Select(t => new TourDto(
                    t.Id,
                    t.Name,
                    t.StartDate,
                    t.EndDate,
                    t.Price,
                    t.AvailablePlaces,
                    t.CountryId,
                    t.Country!.Name,
                    t.ImagePath
                ))
                .ToListAsync();

            return Results.Ok(tours);
        });

        // GET: Получить тур по ID
        group.MapGet("/{id:int}", async (
            int id,
            AppDbContext db) =>
        {
            var tour = await db.Tours
                .Where(t => t.Id == id)
                .Select(t => new TourDto(
                    t.Id,
                    t.Name,
                    t.StartDate,
                    t.EndDate,
                    t.Price,
                    t.AvailablePlaces,
                    t.CountryId,
                    t.Country!.Name,
                    t.ImagePath
                ))
                .FirstOrDefaultAsync();

            if (tour is null)
                return Results.NotFound();

            return Results.Ok(tour);
        });
        
// POST: Создать новый тур
group.MapPost("/", async (
    CreateTourDto dto,
    AppDbContext db) =>
{
    // Проверяем название
    if (string.IsNullOrWhiteSpace(dto.Name))
        return Results.BadRequest("Название тура обязательно.");

    // Проверяем цену
    if (dto.Price <= 0)
        return Results.BadRequest("Цена должна быть больше нуля.");

    // Проверяем количество мест
    if (dto.AvailablePlaces <= 0)
        return Results.BadRequest("Количество мест должно быть больше нуля.");

    // Проверяем даты
    if (dto.StartDate.Date >= dto.EndDate.Date)
        return Results.BadRequest("Дата окончания должна быть позже даты начала.");

    // Особое правило: тур не длиннее 30 дней
    if ((dto.EndDate.Date - dto.StartDate.Date).TotalDays > 30)
        return Results.BadRequest("Продолжительность тура не может превышать 30 дней.");

    // Проверяем существование страны
    var countryExists = await db.Countries
        .AnyAsync(c => c.Id == dto.CountryId);

    if (!countryExists)
        return Results.BadRequest("Указанная страна не существует.");

    // Создаём тур
    var tour = new TravelApi.Models.Tour
    {
        Name = dto.Name,
        StartDate = dto.StartDate,
        EndDate = dto.EndDate,
        Price = dto.Price,
        AvailablePlaces = dto.AvailablePlaces,
        CountryId = dto.CountryId,
        AuthorId = 1
    };

    db.Tours.Add(tour);
    await db.SaveChangesAsync();

    // Формируем DTO для ответа
    var result = await db.Tours
        .Where(t => t.Id == tour.Id)
        .Select(t => new TourDto(
            t.Id,
            t.Name,
            t.StartDate,
            t.EndDate,
            t.Price,
            t.AvailablePlaces,
            t.CountryId,
            t.Country!.Name,
            t.ImagePath
        ))
        .FirstAsync();

    return Results.Created($"/api/tours/{tour.Id}", result);
});

// PUT: Изменить существующий тур
group.MapPut("/{id:int}", async (
    int id,
    UpdateTourDto dto,
    AppDbContext db) =>
{
    var tour = await db.Tours.FindAsync(id);

    if (tour is null)
        return Results.NotFound("Тур не найден.");

    if (string.IsNullOrWhiteSpace(dto.Name))
        return Results.BadRequest("Название тура обязательно.");

    if (dto.Price <= 0)
        return Results.BadRequest("Цена должна быть больше нуля.");

    if (dto.AvailablePlaces <= 0)
        return Results.BadRequest("Количество мест должно быть больше нуля.");

    if (dto.StartDate.Date >= dto.EndDate.Date)
        return Results.BadRequest("Дата окончания должна быть позже даты начала.");

    // Правило: тур не длиннее 30 дней
    if ((dto.EndDate.Date - dto.StartDate.Date).TotalDays > 30)
        return Results.BadRequest("Продолжительность тура не может превышать 30 дней.");

    var countryExists = await db.Countries
        .AnyAsync(c => c.Id == dto.CountryId);

    if (!countryExists)
        return Results.BadRequest("Указанная страна не существует.");

    // Обновляем данные
    tour.Name = dto.Name;
    tour.StartDate = dto.StartDate;
    tour.EndDate = dto.EndDate;
    tour.Price = dto.Price;
    tour.AvailablePlaces = dto.AvailablePlaces;
    tour.CountryId = dto.CountryId;

    await db.SaveChangesAsync();

    var result = await db.Tours
        .Where(t => t.Id == id)
        .Select(t => new TourDto(
            t.Id,
            t.Name,
            t.StartDate,
            t.EndDate,
            t.Price,
            t.AvailablePlaces,
            t.CountryId,
            t.Country!.Name,
            t.ImagePath
        ))
        .FirstAsync();

    return Results.Ok(result);
});

// DELETE: Удалить тур
group.MapDelete("/{id:int}", async (
    int id,
    AppDbContext db) =>
{
    var tour = await db.Tours.FindAsync(id);

    if (tour is null)
        return Results.NotFound("Тур не найден.");

    db.Tours.Remove(tour);

    await db.SaveChangesAsync();

    return Results.NoContent();
});



        return group;
    }
}
