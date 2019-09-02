using Microsoft.EntityFrameworkCore.Migrations;

namespace SaphirCloudBox.Data.Migrations
{
    public partial class FileStorageIsActive : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "FileStorage",
                nullable: false,
                defaultValue: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "FileStorage");
        }
    }
}
