using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixTPHInheritance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FileSystemItems_FileItem_FileItemId",
                table: "FileSystemItems"
            );

            migrationBuilder.DropForeignKey(
                name: "FK_FileSystemItems_FolderItems_ParentFolderId",
                table: "FileSystemItems"
            );

            migrationBuilder.DropTable(name: "FileItem");

            migrationBuilder.DropTable(name: "FolderItems");

            migrationBuilder.DropIndex(
                name: "IX_FileSystemItems_FileItemId",
                table: "FileSystemItems"
            );

            migrationBuilder.DropColumn(name: "FileItemId", table: "FileSystemItems");

            migrationBuilder.AddColumn<string>(
                name: "ContentType",
                table: "FileSystemItems",
                type: "TEXT",
                maxLength: 100,
                nullable: true
            );

            migrationBuilder.AddColumn<string>(
                name: "StorageFileName",
                table: "FileSystemItems",
                type: "TEXT",
                maxLength: 100,
                nullable: true
            );

            migrationBuilder.AddForeignKey(
                name: "FK_FileSystemItems_FileSystemItems_ParentFolderId",
                table: "FileSystemItems",
                column: "ParentFolderId",
                principalTable: "FileSystemItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FileSystemItems_FileSystemItems_ParentFolderId",
                table: "FileSystemItems"
            );

            migrationBuilder.DropColumn(name: "ContentType", table: "FileSystemItems");

            migrationBuilder.DropColumn(name: "StorageFileName", table: "FileSystemItems");

            migrationBuilder.AddColumn<Guid>(
                name: "FileItemId",
                table: "FileSystemItems",
                type: "TEXT",
                nullable: true
            );

            migrationBuilder.CreateTable(
                name: "FileItem",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ContentType = table.Column<string>(
                        type: "TEXT",
                        maxLength: 100,
                        nullable: false
                    ),
                    StorageFileName = table.Column<string>(
                        type: "TEXT",
                        maxLength: 100,
                        nullable: false
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FileItem_FileSystemItems_Id",
                        column: x => x.Id,
                        principalTable: "FileSystemItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "FolderItems",
                columns: table => new { Id = table.Column<Guid>(type: "TEXT", nullable: false) },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FolderItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FolderItems_FileSystemItems_Id",
                        column: x => x.Id,
                        principalTable: "FileSystemItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateIndex(
                name: "IX_FileSystemItems_FileItemId",
                table: "FileSystemItems",
                column: "FileItemId"
            );

            migrationBuilder.AddForeignKey(
                name: "FK_FileSystemItems_FileItem_FileItemId",
                table: "FileSystemItems",
                column: "FileItemId",
                principalTable: "FileItem",
                principalColumn: "Id"
            );

            migrationBuilder.AddForeignKey(
                name: "FK_FileSystemItems_FolderItems_ParentFolderId",
                table: "FileSystemItems",
                column: "ParentFolderId",
                principalTable: "FolderItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull
            );
        }
    }
}
