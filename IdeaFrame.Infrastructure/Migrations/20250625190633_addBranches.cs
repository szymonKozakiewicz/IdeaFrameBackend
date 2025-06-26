using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IdeaFrame.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addBranches : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Branches",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TargetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SourceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Branches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Branches_MindMapNodes_SourceId",
                        column: x => x.SourceId,
                        principalTable: "MindMapNodes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Branches_MindMapNodes_TargetId",
                        column: x => x.TargetId,
                        principalTable: "MindMapNodes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Branches_SourceId",
                table: "Branches",
                column: "SourceId");

            migrationBuilder.CreateIndex(
                name: "IX_Branches_TargetId",
                table: "Branches",
                column: "TargetId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Branches");
        }
    }
}
