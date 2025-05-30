using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IdeaFrame.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MindMapNode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MindMapNodes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    positionX = table.Column<int>(type: "int", nullable: false),
                    positionY = table.Column<int>(type: "int", nullable: false),
                    FileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MindMapNodes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MindMapNodes_FileSystemItems_FileId",
                        column: x => x.FileId,
                        principalTable: "FileSystemItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MindMapNodes_FileId",
                table: "MindMapNodes",
                column: "FileId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MindMapNodes");
        }
    }
}
