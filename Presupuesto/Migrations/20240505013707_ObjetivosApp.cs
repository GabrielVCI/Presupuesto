using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Presupuesto.Migrations
{
    /// <inheritdoc />
    public partial class ObjetivosApp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ObjetivosId",
                table: "Trabajo",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Objetivos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreObjetivo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ObjetivoMonetario = table.Column<int>(type: "int", nullable: false),
                    FechaLimite = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Objetivos", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Trabajo_ObjetivosId",
                table: "Trabajo",
                column: "ObjetivosId");

            migrationBuilder.AddForeignKey(
                name: "FK_Trabajo_Objetivos_ObjetivosId",
                table: "Trabajo",
                column: "ObjetivosId",
                principalTable: "Objetivos",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Trabajo_Objetivos_ObjetivosId",
                table: "Trabajo");

            migrationBuilder.DropTable(
                name: "Objetivos");

            migrationBuilder.DropIndex(
                name: "IX_Trabajo_ObjetivosId",
                table: "Trabajo");

            migrationBuilder.DropColumn(
                name: "ObjetivosId",
                table: "Trabajo");
        }
    }
}
