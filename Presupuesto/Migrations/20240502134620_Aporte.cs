using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Presupuesto.Migrations
{
    /// <inheritdoc />
    public partial class Aporte : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Aportes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Cantidad = table.Column<int>(type: "int", nullable: false),
                    AporteId = table.Column<int>(type: "int", nullable: false),
                    PersonaId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Aportes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Aportes_Trabajo_PersonaId",
                        column: x => x.PersonaId,
                        principalTable: "Trabajo",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Aportes_PersonaId",
                table: "Aportes",
                column: "PersonaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Aportes");
        }
    }
}
