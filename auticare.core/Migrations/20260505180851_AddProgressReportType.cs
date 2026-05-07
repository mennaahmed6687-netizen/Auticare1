using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace auticare.core.Migrations
{
    /// <inheritdoc />
    public partial class AddProgressReportType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ReportType",
                table: "ProgressReports",
                newName: "ProgressReportType");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ProgressReportType",
                table: "ProgressReports",
                newName: "ReportType");
        }
    }
}
