using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IdeaFrame.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class filesIsolationBetweenUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "OwnerId",
                table: "FileSystemItems",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_FileSystemItems_OwnerId",
                table: "FileSystemItems",
                column: "OwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_FileSystemItems_AspNetUsers_OwnerId",
                table: "FileSystemItems",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FileSystemItems_AspNetUsers_OwnerId",
                table: "FileSystemItems");

            migrationBuilder.DropIndex(
                name: "IX_FileSystemItems_OwnerId",
                table: "FileSystemItems");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "FileSystemItems");
        }
    }
}
