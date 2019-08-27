using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SaphirCloudBox.Data.Migrations
{
    public partial class UpdatePermission : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_FileStoragePermission",
                table: "FileStoragePermission");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "FileStoragePermission",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_FileStoragePermission",
                table: "FileStoragePermission",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_FileStoragePermission_FileStorageId",
                table: "FileStoragePermission",
                column: "FileStorageId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_FileStoragePermission",
                table: "FileStoragePermission");

            migrationBuilder.DropIndex(
                name: "IX_FileStoragePermission_FileStorageId",
                table: "FileStoragePermission");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "FileStoragePermission");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FileStoragePermission",
                table: "FileStoragePermission",
                columns: new[] { "FileStorageId", "RecipientId" });
        }
    }
}
