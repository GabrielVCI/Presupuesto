using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Presupuesto.Migrations
{
    /// <inheritdoc />
    public partial class Transaccion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Cantidad_personas",
                table: "Trabajo",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Transacciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Codigo_transaccion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PersonaId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    personasId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transacciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Transacciones_Trabajo_personasId",
                        column: x => x.personasId,
                        principalTable: "Trabajo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Transacciones_personasId",
                table: "Transacciones",
                column: "personasId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Transacciones");

            migrationBuilder.DropColumn(
                name: "Cantidad_personas",
                table: "Trabajo");
        }
    }
}
