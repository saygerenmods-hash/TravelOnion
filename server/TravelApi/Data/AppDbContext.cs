
using Microsoft.EntityFrameworkCore;
using TravelApi.Models;

namespace TravelApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    // Таблицы базы данных
    public DbSet<Tour> Tours => Set<Tour>();
    public DbSet<Country> Countries => Set<Country>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Связь: одна страна — много туров
        modelBuilder.Entity<Tour>()
            .HasOne(t => t.Country)
            .WithMany(c => c.Tours)
            .HasForeignKey(t => t.CountryId)
            .OnDelete(DeleteBehavior.Restrict);

        // Начальные страны
        modelBuilder.Entity<Country>().HasData(
            new Country { Id = 1, Name = "Турция" },
            new Country { Id = 2, Name = "Франция" },
            new Country { Id = 3, Name = "Япония" },
            new Country { Id = 4, Name = "Италия" },
            new Country { Id = 5, Name = "Испания" }
        );

        // Начальные туры
        modelBuilder.Entity<Tour>().HasData(
            new Tour
            {
                Id = 1,
                Name = "Отдых в Анталии",
                StartDate = new DateTime(2026, 12, 1),
                EndDate = new DateTime(2026, 12, 10),
                Price = 850,
                AvailablePlaces = 15,
                CountryId = 1,
                AuthorId = 1
            },
            new Tour
            {
                Id = 2,
                Name = "Парижские каникулы",
                StartDate = new DateTime(2026, 12, 5),
                EndDate = new DateTime(2026, 12, 12),
                Price = 1200,
                AvailablePlaces = 8,
                CountryId = 2,
                AuthorId = 1
            },
            new Tour
            {
                Id = 3,
                Name = "Путешествие по Токио",
                StartDate = new DateTime(2026, 12, 10),
                EndDate = new DateTime(2026, 12, 22),
                Price = 1800,
                AvailablePlaces = 10,
                CountryId = 3,
                AuthorId = 1
            },
            new Tour
            {
                Id = 4,
                Name = "Римские каникулы",
                StartDate = new DateTime(2026, 12, 15),
                EndDate = new DateTime(2026, 12, 22),
                Price = 950,
                AvailablePlaces = 12,
                CountryId = 4,
                AuthorId = 1
            },
            new Tour
            {
                Id = 5,
                Name = "Отдых в Барселоне",
                StartDate = new DateTime(2026, 12, 20),
                EndDate = new DateTime(2026, 12, 30),
                Price = 1100,
                AvailablePlaces = 20,
                CountryId = 5,
                AuthorId = 1
            }
        );
    }
}
