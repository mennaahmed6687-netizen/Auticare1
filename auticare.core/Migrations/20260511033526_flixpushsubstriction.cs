using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace auticare.core.Migrations
{
    /// <inheritdoc />
    public partial class flixpushsubstriction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_AspNetUsers_ParentId",
                table: "Appointments");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "PushSubscriptions",
                newName: "Id");

            migrationBuilder.AlterColumn<string>(
                name: "ParentId",
                table: "Appointments",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_AspNetUsers_ParentId",
                table: "Appointments",
                column: "ParentId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_AspNetUsers_ParentId",
                table: "Appointments");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "PushSubscriptions",
                newName: "id");

            migrationBuilder.AlterColumn<string>(
                name: "ParentId",
                table: "Appointments",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_AspNetUsers_ParentId",
                table: "Appointments",
                column: "ParentId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
