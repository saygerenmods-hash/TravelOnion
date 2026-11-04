
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TravelApi.Data;
using TravelApi.DTOs;
using TravelApi.Models;

namespace TravelApi.Services;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/auth")
            .WithTags("Auth");

        // РЕГИСТРАЦИЯ
        group.MapPost("/register", async (
            RegisterDto dto,
            AppDbContext db,
            IValidator<RegisterDto> validator,
            IPasswordHasher<User> passwordHasher) =>
        {
            var validation = await validator.ValidateAsync(dto);

            if (!validation.IsValid)
            {
                var errors = validation.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.ErrorMessage).ToArray()
                    );

                return Results.ValidationProblem(errors);
            }

            var username = dto.Username.Trim();
            var email = dto.Email.Trim().ToLowerInvariant();

            if (await db.Users.AnyAsync(u => u.Username == username))
            {
                return Results.ValidationProblem(
                    new Dictionary<string, string[]>
                    {
                        ["Username"] = ["Это имя пользователя уже занято"]
                    });
            }

            if (await db.Users.AnyAsync(u => u.Email == email))
            {
                return Results.ValidationProblem(
                    new Dictionary<string, string[]>
                    {
                        ["Email"] = ["Этот email уже зарегистрирован"]
                    });
            }

            var user = new User
            {
                Username = username,
                Email = email,
                CreatedAt = DateTime.UtcNow
            };

            user.PasswordHash = passwordHasher.HashPassword(
                user, dto.Password);

            db.Users.Add(user);
            await db.SaveChangesAsync();

            var result = new UserDto(
                user.Id,
                user.Username,
                user.Email
            );

            return Results.Created(
                $"/api/users/{user.Id}", result);
        });

        // ВХОД
        group.MapPost("/login", async (
            LoginDto dto,
            AppDbContext db,
            IPasswordHasher<User> passwordHasher,
            JwtService jwtService) =>
        {
            if (string.IsNullOrWhiteSpace(dto.Email) ||
                string.IsNullOrWhiteSpace(dto.Password))
            {
                return Results.ValidationProblem(
                    new Dictionary<string, string[]>
                    {
                        ["Login"] = ["Введите email и пароль"]
                    });
            }

            var email = dto.Email.Trim().ToLowerInvariant();

            var user = await db.Users
                .FirstOrDefaultAsync(u => u.Email == email);

            if (user is null)
            {
                return Results.Unauthorized();
            }

            var passwordResult =
                passwordHasher.VerifyHashedPassword(
                    user,
                    user.PasswordHash,
                    dto.Password
                );

            if (passwordResult == PasswordVerificationResult.Failed)
            {
                return Results.Unauthorized();
            }

            var response = jwtService.GenerateToken(user);

            return Results.Ok(response);
        });
    }
}
