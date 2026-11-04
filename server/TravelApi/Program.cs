
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
builder.Services.AddScoped<IValidator<CreateTourDto>, CreateTourValidator>();
builder.Services.AddScoped<IValidator<UpdateTourDto>, UpdateTourValidator>();

// Поддержка ответов ProblemDetails
builder.Services.AddProblemDetails();

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

app.MapTourEndpoints();

app.Run();
