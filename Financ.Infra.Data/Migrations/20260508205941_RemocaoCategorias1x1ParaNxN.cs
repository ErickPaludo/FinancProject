using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Financ.Infra.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemocaoCategorias1x1ParaNxN : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_fnc_movimentacoes_fnc_categorias_IdCategoria",
                table: "fnc_movimentacoes");

            migrationBuilder.DropIndex(
                name: "IX_fnc_movimentacoes_IdCategoria",
                table: "fnc_movimentacoes");

            migrationBuilder.DropColumn(
                name: "IdCategoria",
                table: "fnc_movimentacoes");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IdCategoria",
                table: "fnc_movimentacoes",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_fnc_movimentacoes_IdCategoria",
                table: "fnc_movimentacoes",
                column: "IdCategoria");

            migrationBuilder.AddForeignKey(
                name: "FK_fnc_movimentacoes_fnc_categorias_IdCategoria",
                table: "fnc_movimentacoes",
                column: "IdCategoria",
                principalTable: "fnc_categorias",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
