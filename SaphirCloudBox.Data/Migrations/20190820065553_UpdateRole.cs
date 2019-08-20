using Microsoft.EntityFrameworkCore.Migrations;

namespace SaphirCloudBox.Data.Migrations
{
    public partial class UpdateRole : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RoleType",
                table: "AspNetRoles",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RoleType",
                table: "AspNetRoles");
        }
    }
}
