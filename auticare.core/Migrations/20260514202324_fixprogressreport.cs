using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace auticare.core.Migrations
{
    /// <inheritdoc />
    public partial class fixprogressreport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProgressReports_Childerns_ChildId",
                table: "ProgressReports");

            migrationBuilder.DropIndex(
                name: "IX_ProgressReports_ChildId",
                table: "ProgressReports");

            migrationBuilder.DropColumn(
                name: "ChildId",
                table: "ProgressReports");

            migrationBuilder.AddColumn<int>(
                name: "ChildernChildId",
                table: "ProgressReports",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProgressReports_ChildernChildId",
                table: "ProgressReports",
                column: "ChildernChildId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProgressReports_Childerns_ChildernChildId",
                table: "ProgressReports",
                column: "ChildernChildId",
                principalTable: "Childerns",
                principalColumn: "ChildId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProgressReports_Childerns_ChildernChildId",
                table: "ProgressReports");

            migrationBuilder.DropIndex(
                name: "IX_ProgressReports_ChildernChildId",
                table: "ProgressReports");

            migrationBuilder.DropColumn(
                name: "ChildernChildId",
                table: "ProgressReports");

            migrationBuilder.AddColumn<int>(
                name: "ChildId",
                table: "ProgressReports",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ProgressReports_ChildId",
                table: "ProgressReports",
                column: "ChildId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProgressReports_Childerns_ChildId",
                table: "ProgressReports",
                column: "ChildId",
                principalTable: "Childerns",
                principalColumn: "ChildId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
