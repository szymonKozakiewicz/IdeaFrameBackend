using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IdeaFrame.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MindMapNodeAddColor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "positionY",
                table: "MindMapNodes",
                newName: "PositionY");

            migrationBuilder.RenameColumn(
                name: "positionX",
                table: "MindMapNodes",
                newName: "PositionX");

            migrationBuilder.AddColumn<string>(
                name: "Color",
                table: "MindMapNodes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Color",
                table: "MindMapNodes");

            migrationBuilder.RenameColumn(
                name: "PositionY",
                table: "MindMapNodes",
                newName: "positionY");

            migrationBuilder.RenameColumn(
                name: "PositionX",
                table: "MindMapNodes",
                newName: "positionX");
        }
    }
}
