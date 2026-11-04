using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TravelApi.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Countries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Countries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tours",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    StartDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EndDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Price = table.Column<decimal>(type: "TEXT", nullable: false),
                    AvailablePlaces = table.Column<int>(type: "INTEGER", nullable: false),
                    CountryId = table.Column<int>(type: "INTEGER", nullable: false),
                    ImagePath = table.Column<string>(type: "TEXT", nullable: true),
                    AuthorId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tours", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tours_Countries_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Countries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Countries",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Турция" },
                    { 2, "Франция" },
                    { 3, "Япония" },
                    { 4, "Италия" },
                    { 5, "Испания" }
                });

            migrationBuilder.InsertData(
                table: "Tours",
                columns: new[] { "Id", "AuthorId", "AvailablePlaces", "CountryId", "EndDate", "ImagePath", "Name", "Price", "StartDate" },
                values: new object[,]
                {
                    { 1, 1, 15, 1, new DateTime(2026, 12, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Отдых в Анталии", 850m, new DateTime(2026, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 2, 1, 8, 2, new DateTime(2026, 12, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Парижские каникулы", 1200m, new DateTime(2026, 12, 5, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 3, 1, 10, 3, new DateTime(2026, 12, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Путешествие по Токио", 1800m, new DateTime(2026, 12, 10, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 4, 1, 12, 4, new DateTime(2026, 12, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Римские каникулы", 950m, new DateTime(2026, 12, 15, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 5, 1, 20, 5, new DateTime(2026, 12, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Отдых в Барселоне", 1100m, new DateTime(2026, 12, 20, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tours_CountryId",
                table: "Tours",
                column: "CountryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Tours");

            migrationBuilder.DropTable(
                name: "Countries");
        }
    }
}
