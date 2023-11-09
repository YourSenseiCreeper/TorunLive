using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TorunLive.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Directions",
                columns: table => new
                {
                    LineId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DirectionId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Directions", x => new { x.LineId, x.DirectionId });
                });

            migrationBuilder.CreateTable(
                name: "Lines",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lines", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Stops",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stops", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LineStops",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LineId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DirectionId = table.Column<int>(type: "int", nullable: false),
                    StopId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    StopOrder = table.Column<int>(type: "int", nullable: false),
                    IsOnDemand = table.Column<bool>(type: "bit", nullable: false),
                    TimeToNextStop = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LineStops", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LineStops_Lines_LineId",
                        column: x => x.LineId,
                        principalTable: "Lines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LineStops_Stops_StopId",
                        column: x => x.StopId,
                        principalTable: "Stops",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LineStopTimes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LineStopId = table.Column<int>(type: "int", nullable: false),
                    DayMinute = table.Column<int>(type: "int", nullable: false),
                    IsWeekday = table.Column<bool>(type: "bit", nullable: false),
                    IsWinterHoliday = table.Column<bool>(type: "bit", nullable: false),
                    IsSaturdaySundayHoliday = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LineStopTimes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LineStopTimes_LineStops_LineStopId",
                        column: x => x.LineStopId,
                        principalTable: "LineStops",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LineStop_LineStop",
                table: "LineStops",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_LineStops_LineId_DirectionId_StopId",
                table: "LineStops",
                columns: new[] { "LineId", "DirectionId", "StopId" });

            migrationBuilder.CreateIndex(
                name: "IX_LineStops_StopId",
                table: "LineStops",
                column: "StopId");

            migrationBuilder.CreateIndex(
                name: "IX_LineStopTimes_LineStopId",
                table: "LineStopTimes",
                column: "LineStopId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Directions");

            migrationBuilder.DropTable(
                name: "LineStopTimes");

            migrationBuilder.DropTable(
                name: "LineStops");

            migrationBuilder.DropTable(
                name: "Lines");

            migrationBuilder.DropTable(
                name: "Stops");
        }
    }
}
