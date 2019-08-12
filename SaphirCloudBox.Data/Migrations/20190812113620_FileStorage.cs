using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SaphirCloudBox.Data.Migrations
{
    public partial class FileStorage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FileStorage",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IsDirectory = table.Column<bool>(nullable: false),
                    Name = table.Column<string>(maxLength: 255, nullable: false),
                    Extension = table.Column<string>(maxLength: 100, nullable: true),
                    AccessType = table.Column<int>(nullable: false),
                    CreateDate = table.Column<DateTime>(nullable: false),
                    UpdateDate = table.Column<DateTime>(nullable: true),
                    ParentFileStorageId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileStorage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FileStorage_FileStorage_ParentFileStorageId",
                        column: x => x.ParentFileStorageId,
                        principalTable: "FileStorage",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FileStorageAccess",
                columns: table => new
                {
                    FileStorageId = table.Column<int>(nullable: false),
                    UserId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileStorageAccess", x => new { x.FileStorageId, x.UserId });
                    table.ForeignKey(
                        name: "FK_FileStorageAccess_FileStorage_FileStorageId",
                        column: x => x.FileStorageId,
                        principalTable: "FileStorage",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FileStorageAccess_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "FileStorage",
                columns: new[] { "Id", "AccessType", "CreateDate", "Extension", "IsDirectory", "Name", "ParentFileStorageId", "UpdateDate" },
                values: new object[] { 1, 0, new DateTime(2019, 8, 12, 14, 36, 20, 57, DateTimeKind.Local).AddTicks(1724), null, true, "root", null, null });

            migrationBuilder.InsertData(
                table: "FileStorage",
                columns: new[] { "Id", "AccessType", "CreateDate", "Extension", "IsDirectory", "Name", "ParentFileStorageId", "UpdateDate" },
                values: new object[,]
                {
                    { 2, 0, new DateTime(2019, 8, 12, 14, 36, 20, 57, DateTimeKind.Local).AddTicks(6374), null, true, "Saphir", 1, null },
                    { 3, 0, new DateTime(2019, 8, 12, 14, 36, 20, 57, DateTimeKind.Local).AddTicks(6814), null, true, "Автопарк", 1, null },
                    { 4, 0, new DateTime(2019, 8, 12, 14, 36, 20, 57, DateTimeKind.Local).AddTicks(6822), null, true, "Контроль времени", 1, null },
                    { 5, 2, new DateTime(2019, 8, 12, 14, 36, 20, 57, DateTimeKind.Local).AddTicks(6823), null, true, "Моя папка", 1, null },
                    { 6, 0, new DateTime(2019, 8, 12, 14, 36, 20, 57, DateTimeKind.Local).AddTicks(6823), null, true, "Общая информация", 1, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_FileStorage_ParentFileStorageId",
                table: "FileStorage",
                column: "ParentFileStorageId");

            migrationBuilder.CreateIndex(
                name: "IX_FileStorageAccess_UserId",
                table: "FileStorageAccess",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FileStorageAccess");

            migrationBuilder.DropTable(
                name: "FileStorage");
        }
    }
}
