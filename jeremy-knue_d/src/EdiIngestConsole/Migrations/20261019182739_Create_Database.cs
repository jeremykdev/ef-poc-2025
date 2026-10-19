using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EdiIngestConsole.Migrations
{
    /// <inheritdoc />
    public partial class Create_Database : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Isa",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AuthorizationInformationQualifier_1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AuthorizationInformation_2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityInformationQualifier_3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityInformation_4 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SenderIDQualifier_5 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InterchangeSenderID_6 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReceiverIDQualifier_7 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InterchangeReceiverID_8 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InterchangeDate_9 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InterchangeTime_10 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InterchangeControlStandardsIdentifier_11 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InterchangeControlVersionNumber_12 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InterchangeControlNumber_13 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AcknowledgementRequested_14 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UsageIndicator_15 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ComponentElementSeparator_16 = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Isa", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Isa");
        }
    }
}
