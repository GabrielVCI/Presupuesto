using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Presupuesto.Migrations
{
    /// <inheritdoc />
    public partial class apdTransaccioness : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PersonaId",
                table: "Transacciones");

            migrationBuilder.AddColumn<int>(
                name: "PersonaIds",
                table: "Transacciones",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PersonaIds",
                table: "Transacciones");

            migrationBuilder.AddColumn<string>(
                name: "PersonaId",
                table: "Transacciones",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
