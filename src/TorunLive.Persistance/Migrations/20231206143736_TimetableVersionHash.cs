using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TorunLive.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class TimetableVersionHash : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TimeToNextStop",
                table: "LineStops");

            migrationBuilder.AddColumn<string>(
                name: "TimetableVersionHash",
                table: "LineStops",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TimetableVersionHash",
                table: "LineStops");

            migrationBuilder.AddColumn<int>(
                name: "TimeToNextStop",
                table: "LineStops",
                type: "int",
                nullable: true);
        }
    }
}
