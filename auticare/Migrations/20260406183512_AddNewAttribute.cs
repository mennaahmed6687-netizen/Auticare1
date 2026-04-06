using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace auticare.Migrations
{
    /// <inheritdoc />
    public partial class AddNewAttribute : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AudioName",
                table: "Activities",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ImageName",
                table: "Activities",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AudioName",
                table: "Activities");

            migrationBuilder.DropColumn(
                name: "ImageName",
                table: "Activities");
        }
    }
}
