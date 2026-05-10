using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Financ.Infra.Data.Migrations
{
    /// <inheritdoc />
    public partial class movcomcollectiondemovcat2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_fnc_movimentacao_categorias_fnc_movimentacoes_MovimentacaoId",
                table: "fnc_movimentacao_categorias");

            migrationBuilder.DropIndex(
                name: "IX_fnc_movimentacao_categorias_MovimentacaoId",
                table: "fnc_movimentacao_categorias");

            migrationBuilder.DropColumn(
                name: "MovimentacaoId",
                table: "fnc_movimentacao_categorias");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MovimentacaoId",
                table: "fnc_movimentacao_categorias",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_fnc_movimentacao_categorias_MovimentacaoId",
                table: "fnc_movimentacao_categorias",
                column: "MovimentacaoId");

            migrationBuilder.AddForeignKey(
                name: "FK_fnc_movimentacao_categorias_fnc_movimentacoes_MovimentacaoId",
                table: "fnc_movimentacao_categorias",
                column: "MovimentacaoId",
                principalTable: "fnc_movimentacoes",
                principalColumn: "Id");
        }
    }
}
