using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Presupuesto.Migrations
{
    /// <inheritdoc />
    public partial class updAportes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Aprobado",
                table: "Aportes",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Aprobado",
                table: "Aportes");
        }
    }
}
