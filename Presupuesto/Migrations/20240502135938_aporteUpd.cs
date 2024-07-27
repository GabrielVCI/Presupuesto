using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Presupuesto.Migrations
{
    /// <inheritdoc />
    public partial class aporteUpd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Aportes_Trabajo_PersonaId",
                table: "Aportes");

            migrationBuilder.DropColumn(
                name: "AporteId",
                table: "Aportes");

            migrationBuilder.AlterColumn<int>(
                name: "PersonaId",
                table: "Aportes",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Aportes_Trabajo_PersonaId",
                table: "Aportes",
                column: "PersonaId",
                principalTable: "Trabajo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Aportes_Trabajo_PersonaId",
                table: "Aportes");

            migrationBuilder.AlterColumn<int>(
                name: "PersonaId",
                table: "Aportes",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "AporteId",
                table: "Aportes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_Aportes_Trabajo_PersonaId",
                table: "Aportes",
                column: "PersonaId",
                principalTable: "Trabajo",
                principalColumn: "Id");
        }
    }
}
