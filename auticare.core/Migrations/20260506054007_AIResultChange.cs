using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace auticare.core.Migrations
{
    /// <inheritdoc />
    public partial class AIResultChange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AIResults_Childerns_ChildernChildId",
                table: "AIResults");

            migrationBuilder.AlterColumn<int>(
                name: "ChildernChildId",
                table: "AIResults",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_AIResults_Childerns_ChildernChildId",
                table: "AIResults",
                column: "ChildernChildId",
                principalTable: "Childerns",
                principalColumn: "ChildId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AIResults_Childerns_ChildernChildId",
                table: "AIResults");

            migrationBuilder.AlterColumn<int>(
                name: "ChildernChildId",
                table: "AIResults",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AIResults_Childerns_ChildernChildId",
                table: "AIResults",
                column: "ChildernChildId",
                principalTable: "Childerns",
                principalColumn: "ChildId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
