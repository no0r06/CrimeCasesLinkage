using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CrimeCasesLinkage.Migrations
{
    /// <inheritdoc />
    public partial class AddVictimEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Title",
                table: "CrimeCases",
                newName: "Method");

            migrationBuilder.RenameColumn(
                name: "CrimeDate",
                table: "CrimeCases",
                newName: "IncidentDate");

            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "CrimeCases",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Victims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AgeMin = table.Column<int>(type: "int", nullable: false),
                    AgeMax = table.Column<int>(type: "int", nullable: false),
                    Gender = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CrimeCaseId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Victims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Victims_CrimeCases_CrimeCaseId",
                        column: x => x.CrimeCaseId,
                        principalTable: "CrimeCases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Victims_CrimeCaseId",
                table: "Victims",
                column: "CrimeCaseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Victims");

            migrationBuilder.DropColumn(
                name: "Location",
                table: "CrimeCases");

            migrationBuilder.RenameColumn(
                name: "Method",
                table: "CrimeCases",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "IncidentDate",
                table: "CrimeCases",
                newName: "CrimeDate");
        }
    }
}
