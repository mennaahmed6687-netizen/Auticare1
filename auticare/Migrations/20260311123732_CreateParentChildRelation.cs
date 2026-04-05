using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace auticare.Migrations
{
    /// <inheritdoc />
    public partial class CreateParentChildRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ParentId",
                table: "Child",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Child_ParentId",
                table: "Child",
                column: "ParentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Child_Parent_ParentId",
                table: "Child",
                column: "ParentId",
                principalTable: "Parent",
                principalColumn: "ParentId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Child_Parent_ParentId",
                table: "Child");

            migrationBuilder.DropIndex(
                name: "IX_Child_ParentId",
                table: "Child");

            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "Child");
        }
    }
}
