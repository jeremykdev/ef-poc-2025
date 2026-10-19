using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EdiIngestConsole.Migrations
{
    /// <inheritdoc />
    public partial class Add_Ge_And_Iea : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GeEntities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NumberOfIncludedSets_1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GroupControlNumber_2 = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GeEntities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IeaEntities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NumberOfIncludedGroups_1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InterchangeControlNumber_2 = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IeaEntities", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GeEntities");

            migrationBuilder.DropTable(
                name: "IeaEntities");
        }
    }
}
