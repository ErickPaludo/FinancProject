using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Financ.Infra.Data.Migrations
{
    /// <inheritdoc />
    public partial class contaFavorita : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ContaFavorita",
                table: "fnc_contas_usuarios",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Favorito",
                table: "fnc_contas",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContaFavorita",
                table: "fnc_contas_usuarios");

            migrationBuilder.DropColumn(
                name: "Favorito",
                table: "fnc_contas");
        }
    }
}
