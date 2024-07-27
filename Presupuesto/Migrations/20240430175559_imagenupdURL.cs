using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Presupuesto.Migrations
{
    /// <inheritdoc />
    public partial class imagenupdURL : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Url",
                table: "ImagenTransaccion");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Url",
                table: "ImagenTransaccion",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
