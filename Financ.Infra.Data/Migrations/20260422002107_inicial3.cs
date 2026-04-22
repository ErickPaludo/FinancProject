using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Financ.Infra.Data.Migrations
{
    /// <inheritdoc />
    public partial class inicial3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_fnc_movimentacoes_fnc_categorias_IdCategoria",
                table: "fnc_movimentacoes");

            migrationBuilder.AddForeignKey(
                name: "FK_fnc_movimentacoes_fnc_categorias_IdCategoria",
                table: "fnc_movimentacoes",
                column: "IdCategoria",
                principalTable: "fnc_categorias",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_fnc_movimentacoes_fnc_categorias_IdCategoria",
                table: "fnc_movimentacoes");

            migrationBuilder.AddForeignKey(
                name: "FK_fnc_movimentacoes_fnc_categorias_IdCategoria",
                table: "fnc_movimentacoes",
                column: "IdCategoria",
                principalTable: "fnc_categorias",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
