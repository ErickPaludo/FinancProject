using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Financ.Infra.Data.Migrations
{
    /// <inheritdoc />
    public partial class contaFavorita2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Favorito",
                table: "fnc_contas");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Favorito",
                table: "fnc_contas",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
