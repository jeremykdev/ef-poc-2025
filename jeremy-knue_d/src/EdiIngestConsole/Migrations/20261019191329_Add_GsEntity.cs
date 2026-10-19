using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EdiIngestConsole.Migrations
{
    /// <inheritdoc />
    public partial class Add_GsEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Gs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CodeIdentifyingInformationType_1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SenderIDCode_2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReceiverIDCode_3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Date_4 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Time_5 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GroupControlNumber_6 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TransactionTypeCode_7 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VersionAndRelease_8 = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Gs", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Gs");
        }
    }
}
