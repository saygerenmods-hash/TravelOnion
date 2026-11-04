
using System.Security.Claims;
using FluentValidation;
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
        
group.MapGet("/", async (
    AppDbContext db,
    int? countryId,
    decimal? maxPrice,
    bool currentMonth = false,
    int page = 1,
    int pageSize = 5,
    string sortBy = "date") =>
{
    // Начинаем со всех туров
    var query = db.Tours.AsQueryable();

    // Фильтр по стране
    if (countryId.HasValue)
    {
        query = query.Where(t => t.CountryId == countryId.Value);
    }

    // Фильтр по максимальной цене
    if (maxPrice.HasValue)
    {
        query = query.Where(t => t.Price <= maxPrice.Value);
    }

    // Считаем количество подходящих туров
    // Фильтр по текущему месяцу
if (currentMonth)
{
    var now = DateTime.Today;

    var startOfMonth = new DateTime(now.Year, now.Month, 1);
    var startOfNextMonth = startOfMonth.AddMonths(1);

    query = query.Where(t =>
        t.StartDate >= startOfMonth &&
        t.StartDate < startOfNextMonth);
}
    var total = await query.CountAsync();

    // Сортировка
    query = sortBy.ToLower() switch
    {
        "price" => query.OrderBy(t => t.Price),
        "date" => query.OrderBy(t => t.StartDate),
        _ => query.OrderBy(t => t.Id)
    };

    // Ограничиваем размер страницы
    page = Math.Max(1, page);
    pageSize = Math.Clamp(pageSize, 1, 50);

    // Получаем только нужную страницу
    var tours = await query
        .Skip((page - 1) * pageSize)
   
.Take(pageSize)
.Select(t => new TourDto(
    t.Id,
    t.Name,
    t.StartDate,
    t.EndDate,
    t.Price,
    t.AvailablePlaces,
    t.CountryId,
    t.Country!.Name,
    t.ImagePath,
    t.AuthorId
))
.ToListAsync();


    return Results.Ok(new
    {
        page,
        pageSize,
        total,
        items = tours
    });
});

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
                    t.ImagePath,
                    t.AuthorId
                ))
                .FirstOrDefaultAsync();

            if (tour is null)
                return Results.NotFound();

            return Results.Ok(tour);
        });
        
// POST: Создать новый тур
group.MapPost("/", async (
    CreateTourDto dto,
    AppDbContext db,
    IValidator<CreateTourDto> validator,
    ClaimsPrincipal user) =>
{
    var validationResult = await validator.ValidateAsync(dto);

    if (!validationResult.IsValid)
    {
        var errors = validationResult.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(e => e.ErrorMessage).ToArray()
            );

        return Results.ValidationProblem(errors);
    }

   
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
        AuthorId = int.Parse(
    user.FindFirstValue(ClaimTypes.NameIdentifier)!)
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
            t.ImagePath,
            t.AuthorId
        ))
        .FirstAsync();

   
    return Results.Created($"/api/tours/{tour.Id}", result);
})
.RequireAuthorization();


        // PUT: Изменить существующий тур


        // PUT: Изменить существующий тур
        group.MapPut("/{id:int}", async (
    int id,
    UpdateTourDto dto,
    AppDbContext db,
    IValidator<UpdateTourDto> validator,
    ClaimsPrincipal user) =>
        {
            // Ищем тур по ID
            var tour = await db.Tours.FindAsync(id);

            if (tour is null)
                return Results.NotFound("Тур не найден.");
                var userId = int.Parse(
    user.FindFirstValue(ClaimTypes.NameIdentifier)!);

    if (tour.AuthorId != userId)
    return Results.Forbid();

            // Проверяем данные через FluentValidation
            var validationResult = await validator.ValidateAsync(dto);

            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.ErrorMessage).ToArray()
                    );

                return Results.ValidationProblem(errors);
            }

            // Проверяем существование страны
            var countryExists = await db.Countries
                .AnyAsync(c => c.Id == dto.CountryId);

            if (!countryExists)
            {
                return Results.ValidationProblem(
                    new Dictionary<string, string[]>
                    {
                        ["CountryId"] = new[]
                        {
                            "Указанная страна не существует."
                        }
                    });
            }

            // Обновляем данные тура
            tour.Name = dto.Name;
            tour.StartDate = dto.StartDate;
            tour.EndDate = dto.EndDate;
            tour.Price = dto.Price;
            tour.AvailablePlaces = dto.AvailablePlaces;
            tour.CountryId = dto.CountryId;

            await db.SaveChangesAsync();

            // Возвращаем обновлённый тур
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
                    t.ImagePath,
                    t.AuthorId
                ))
                .FirstAsync();

           return Results.Ok(result);
})
.RequireAuthorization();


// DELETE: Удалить тур
group.MapDelete("/{id:int}", async (
    int id,
    AppDbContext db,
    ClaimsPrincipal user) =>
{
    var tour = await db.Tours.FindAsync(id);

    if (tour is null)
        return Results.NotFound("Тур не найден.");
        var userId = int.Parse(
    user.FindFirstValue(ClaimTypes.NameIdentifier)!);

if (tour.AuthorId != userId)
    return Results.Forbid();

    db.Tours.Remove(tour);

    await db.SaveChangesAsync();

    return Results.NoContent();
})
.RequireAuthorization();



        return group;
    }
}
