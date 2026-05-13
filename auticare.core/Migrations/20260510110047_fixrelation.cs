using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace auticare.core.Migrations
{
    /// <inheritdoc />
    public partial class fixrelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ProgressReports_ParentId",
                table: "ProgressReports");

            migrationBuilder.CreateIndex(
                name: "IX_ProgressReports_ParentId",
                table: "ProgressReports",
                column: "ParentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ProgressReports_ParentId",
                table: "ProgressReports");

            migrationBuilder.CreateIndex(
                name: "IX_ProgressReports_ParentId",
                table: "ProgressReports",
                column: "ParentId",
                unique: true);
        }
    }
}
