
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using TravelApi.Data;

namespace TravelApi.Services;

public static class TourImageEndpoints
{
    public static RouteGroupBuilder MapTourImageEndpoints(
        this WebApplication app)
    {
        var group = app.MapGroup("/api/tours")
            .WithTags("Tour Images");

        group.MapPost("/{id:int}/image", async (
            int id,
            IFormFile file,
            AppDbContext db,
            ClaimsPrincipal user,
            IWebHostEnvironment environment) =>
        {
            var tour = await db.Tours.FindAsync(id);

            if (tour is null)
                return Results.NotFound("Тур не найден.");

            var userId = int.Parse(
                user.FindFirstValue(ClaimTypes.NameIdentifier)!);

            if (tour.AuthorId != userId)
                return Results.Forbid();

            // Максимум 5 МБ
            const long maxSize = 5 * 1024 * 1024;

            if (file.Length == 0 || file.Length > maxSize)
                return Results.BadRequest(
                    "Размер изображения должен быть от 1 байта до 5 МБ.");

            // Разрешённые форматы
            var allowedTypes = new Dictionary<string, string>
            {
                ["image/jpeg"] = ".jpg",
                ["image/png"] = ".png",
                ["image/webp"] = ".webp"
            };

            if (!allowedTypes.TryGetValue(
                file.ContentType.ToLowerInvariant(),
                out var extension))
            {
                return Results.BadRequest(
                    "Разрешены только JPG, PNG и WebP.");
            }

            var uploadsFolder = Path.Combine(
                environment.ContentRootPath,
                "wwwroot",
                "uploads");

            Directory.CreateDirectory(uploadsFolder);
            
// Проверяем реальные байты изображения,
// а не только ContentType из запроса
var header = new byte[12];

await using (var input = file.OpenReadStream())
{
    var bytesRead = await input.ReadAsync(header);

    if (bytesRead < 12)
        return Results.BadRequest("Некорректный файл изображения.");
}

bool isValidImage = extension switch
{
    ".jpg" =>
        header[0] == 0xFF &&
        header[1] == 0xD8 &&
        header[2] == 0xFF,

    ".png" =>
        header[0] == 0x89 &&
        header[1] == 0x50 &&
        header[2] == 0x4E &&
        header[3] == 0x47 &&
        header[4] == 0x0D &&
        header[5] == 0x0A &&
        header[6] == 0x1A &&
        header[7] == 0x0A,

    ".webp" =>
        header[0] == 0x52 &&
        header[1] == 0x49 &&
        header[2] == 0x46 &&
        header[3] == 0x46 &&
        header[8] == 0x57 &&
        header[9] == 0x45 &&
        header[10] == 0x42 &&
        header[11] == 0x50,

    _ => false
};

if (!isValidImage)
{
    return Results.BadRequest(
        "Содержимое файла не соответствует формату изображения.");
}


            var fileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            await using (var stream = File.Create(filePath))
            {
                await file.CopyToAsync(stream);
            }

           
// Запоминаем старую фотографию
var oldImagePath = tour.ImagePath;

// Сохраняем путь к новой фотографии
tour.ImagePath = $"/uploads/{fileName}";

try
{
    await db.SaveChangesAsync();
}
catch
{
    // Если запись в БД не удалась,
// удаляем только что загруженный файл
    if (File.Exists(filePath))
        File.Delete(filePath);

    throw;
}

// После успешного обновления БД
// удаляем предыдущую фотографию
if (!string.IsNullOrEmpty(oldImagePath))
{
    var oldFileName = Path.GetFileName(oldImagePath);
    var oldFilePath = Path.Combine(uploadsFolder, oldFileName);

    if (File.Exists(oldFilePath))
    {
        File.Delete(oldFilePath);
    }
}


            return Results.Ok(new
            {
                tour.Id,
                tour.ImagePath
            });
        })
        .DisableAntiforgery()
        .RequireAuthorization();

        return group;
    }
}
