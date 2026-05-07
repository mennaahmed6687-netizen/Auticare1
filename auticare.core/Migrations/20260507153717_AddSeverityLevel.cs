using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace auticare.core.Migrations
{
    /// <inheritdoc />
    public partial class AddSeverityLevel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Severity_Level",
                table: "AIResults",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Severity_Level",
                table: "AIResults");
        }
    }
}
