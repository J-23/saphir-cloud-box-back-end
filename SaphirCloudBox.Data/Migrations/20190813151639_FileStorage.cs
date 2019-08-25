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
                    BlobName = table.Column<Guid>(nullable: true),
                    ClientId = table.Column<int>(nullable: true),
                    OwnerId = table.Column<int>(nullable: true),
                    CreateById = table.Column<int>(nullable: false),
                    CreateDate = table.Column<DateTime>(nullable: false),
                    UpdateById = table.Column<int>(nullable: true),
                    UpdateDate = table.Column<DateTime>(nullable: true),
                    ParentFileStorageId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileStorage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FileStorage_Client_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Client",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FileStorage_AspNetUsers_CreateById",
                        column: x => x.CreateById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_FileStorage_AspNetUsers_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FileStorage_FileStorage_ParentFileStorageId",
                        column: x => x.ParentFileStorageId,
                        principalTable: "FileStorage",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FileStorage_AspNetUsers_UpdateById",
                        column: x => x.UpdateById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FileStorageAccessUser",
                columns: table => new
                {
                    FileStorageId = table.Column<int>(nullable: false),
                    UserId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileStorageAccessUser", x => new { x.FileStorageId, x.UserId });
                    table.ForeignKey(
                        name: "FK_FileStorageAccessUser_FileStorage_FileStorageId",
                        column: x => x.FileStorageId,
                        principalTable: "FileStorage",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_FileStorageAccessUser_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.InsertData(
                table: "FileStorage",
                columns: new[] { "Id", "BlobName", "ClientId", "CreateById", "CreateDate", "IsDirectory", "Name", "OwnerId", "ParentFileStorageId", "UpdateById", "UpdateDate" },
                values: new object[] { 1, null, null, 1, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "root", null, null, null, null });

            migrationBuilder.CreateIndex(
                name: "IX_FileStorage_ClientId",
                table: "FileStorage",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_FileStorage_CreateById",
                table: "FileStorage",
                column: "CreateById");

            migrationBuilder.CreateIndex(
                name: "IX_FileStorage_OwnerId",
                table: "FileStorage",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_FileStorage_ParentFileStorageId",
                table: "FileStorage",
                column: "ParentFileStorageId");

            migrationBuilder.CreateIndex(
                name: "IX_FileStorage_UpdateById",
                table: "FileStorage",
                column: "UpdateById");

            migrationBuilder.CreateIndex(
                name: "IX_FileStorageAccessUser_UserId",
                table: "FileStorageAccessUser",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FileStorageAccessUser");

            migrationBuilder.DropTable(
                name: "FileStorage");
        }
    }
}
