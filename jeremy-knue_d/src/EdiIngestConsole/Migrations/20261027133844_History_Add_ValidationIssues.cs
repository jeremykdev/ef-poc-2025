using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EdiIngestConsole.Migrations
{
    /// <inheritdoc />
    public partial class History_Add_ValidationIssues : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ValidationIssues",
                table: "History",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ValidationIssues",
                table: "History");
        }
    }
}
