
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Identity;
using TravelApi.Models;
using FluentValidation;
using TravelApi.DTOs;
using TravelApi.Validators;
using Microsoft.EntityFrameworkCore;
using TravelApi.Data;
using TravelApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Подключение SQLite
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// Регистрируем валидаторы
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<JwtService>();
builder.Services.AddScoped<IValidator<CreateTourDto>, CreateTourValidator>();
builder.Services.AddScoped<IValidator<UpdateTourDto>, UpdateTourValidator>();
builder.Services.AddScoped<IValidator<RegisterDto>, RegisterValidator>();

// Поддержка ответов ProblemDetails
builder.Services.AddProblemDetails();

// Настройка JWT-авторизации
var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException(
        "JWT key is not configured");

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtKey)),

                ValidateIssuer = false,
                ValidateAudience = false,

                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
    });

builder.Services.AddAuthorization();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/", () =>
{
    return Results.Ok(new
    {
        message = "TravelOnion API is running!"
    });
});

app.UseAuthentication();
app.UseAuthorization();
app.MapTourEndpoints();
app.MapAuthEndpoints();

app.Run();
