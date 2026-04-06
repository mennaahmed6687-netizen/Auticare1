using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace auticare.Migrations
{
    /// <inheritdoc />
    public partial class AddProgressReport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProgressReports",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Report = table.Column<int>(type: "int", nullable: true),
                    AverageScore = table.Column<double>(type: "float", nullable: false),
                    ChildId = table.Column<int>(type: "int", nullable: false),
                    RecommendedNextStep = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReportDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OverallProgress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    completedactivities = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProgressReports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProgressReports_Child_ChildId",
                        column: x => x.ChildId,
                        principalTable: "Child",
                        principalColumn: "ChildId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProgressReports_ChildId",
                table: "ProgressReports",
                column: "ChildId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProgressReports");
        }
    }
}
