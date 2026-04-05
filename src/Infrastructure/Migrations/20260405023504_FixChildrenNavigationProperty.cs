using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixChildrenNavigationProperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FileSystemItems_FileSystemItems_FileItemId",
                table: "FileSystemItems");

            migrationBuilder.DropColumn(
                name: "FileItemId",
                table: "FileSystemItems");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "FileItemId",
                table: "FileSystemItems",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_FileSystemItems_FileSystemItems_FileItemId",
                table: "FileSystemItems",
                column: "FileItemId",
                principalTable: "FileSystemItems",
                principalColumn: "Id");
        }
    }
}
