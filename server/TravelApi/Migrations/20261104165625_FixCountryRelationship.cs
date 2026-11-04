using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TravelApi.Migrations
{
    /// <inheritdoc />
    public partial class FixCountryRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tours_Countries_CountryId",
                table: "Tours");

            migrationBuilder.AddForeignKey(
                name: "FK_Tours_Countries_CountryId",
                table: "Tours",
                column: "CountryId",
                principalTable: "Countries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tours_Countries_CountryId",
                table: "Tours");

            migrationBuilder.AddForeignKey(
                name: "FK_Tours_Countries_CountryId",
                table: "Tours",
                column: "CountryId",
                principalTable: "Countries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
