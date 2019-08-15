using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SaphirCloudBox.Data.Migrations
{
    public partial class BlobStorageAndFile : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "File",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    FileStorageId = table.Column<int>(nullable: false),
                    Extension = table.Column<string>(maxLength: 100, nullable: false),
                    Size = table.Column<int>(nullable: false),
                    SizeType = table.Column<string>(maxLength: 2, nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    CreateById = table.Column<int>(nullable: false),
                    CreateDate = table.Column<DateTime>(nullable: false),
                    UpdateById = table.Column<int>(nullable: true),
                    UpdateDate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_File", x => x.Id);
                    table.ForeignKey(
                        name: "FK_File_AspNetUsers_CreateById",
                        column: x => x.CreateById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_File_FileStorage_FileStorageId",
                        column: x => x.FileStorageId,
                        principalTable: "FileStorage",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_File_AspNetUsers_UpdateById",
                        column: x => x.UpdateById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AzureBlobStorage",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BlobName = table.Column<Guid>(nullable: false),
                    FileId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AzureBlobStorage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AzureBlobStorage_File_FileId",
                        column: x => x.FileId,
                        principalTable: "File",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AzureBlobStorage_FileId",
                table: "AzureBlobStorage",
                column: "FileId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_File_CreateById",
                table: "File",
                column: "CreateById");

            migrationBuilder.CreateIndex(
                name: "IX_File_FileStorageId",
                table: "File",
                column: "FileStorageId");

            migrationBuilder.CreateIndex(
                name: "IX_File_UpdateById",
                table: "File",
                column: "UpdateById");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AzureBlobStorage");

            migrationBuilder.DropTable(
                name: "File");
        }
    }
}
