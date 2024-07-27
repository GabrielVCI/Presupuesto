using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Presupuesto.Migrations
{
    /// <inheritdoc />
    public partial class ConfigUsuarios : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UsuarioCreacionId",
                table: "Objetivos",
                type: "nvarchar(450)",
                nullable: true, //this have to be true in order to add a migration and a FK from another table
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Objetivos_UsuarioCreacionId",
                table: "Objetivos",
                column: "UsuarioCreacionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Objetivos_AspNetUsers_UsuarioCreacionId",
                table: "Objetivos",
                column: "UsuarioCreacionId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Objetivos_AspNetUsers_UsuarioCreacionId",
                table: "Objetivos");

            migrationBuilder.DropIndex(
                name: "IX_Objetivos_UsuarioCreacionId",
                table: "Objetivos");

            migrationBuilder.DropColumn(
                name: "UsuarioCreacionId",
                table: "Objetivos");
        }
    }
}
