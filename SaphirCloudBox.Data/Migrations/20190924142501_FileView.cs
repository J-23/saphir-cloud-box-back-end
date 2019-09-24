using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SaphirCloudBox.Data.Migrations
{
    public partial class FileView : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FileViewing",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ViewDate = table.Column<DateTime>(nullable: false),
                    ViewById = table.Column<int>(nullable: false),
                    FileStorageId = table.Column<int>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileViewing", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FileViewing_FileStorage_FileStorageId",
                        column: x => x.FileStorageId,
                        principalTable: "FileStorage",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_FileViewing_AspNetUsers_ViewById",
                        column: x => x.ViewById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FileViewing_FileStorageId",
                table: "FileViewing",
                column: "FileStorageId");

            migrationBuilder.CreateIndex(
                name: "IX_FileViewing_ViewById",
                table: "FileViewing",
                column: "ViewById");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FileViewing");
        }
    }
}
