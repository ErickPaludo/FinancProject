using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Financ.Infra.Data.Migrations
{
    /// <inheritdoc />
    public partial class CampoMovimentacaoEditado2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_fnc_movimentacoes_fnc_movimentacoes_fixas_IdFixo",
                table: "fnc_movimentacoes");

            migrationBuilder.AddForeignKey(
                name: "FK_fnc_movimentacoes_fnc_movimentacoes_fixas_IdFixo",
                table: "fnc_movimentacoes",
                column: "IdFixo",
                principalTable: "fnc_movimentacoes_fixas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_fnc_movimentacoes_fnc_movimentacoes_fixas_IdFixo",
                table: "fnc_movimentacoes");

            migrationBuilder.AddForeignKey(
                name: "FK_fnc_movimentacoes_fnc_movimentacoes_fixas_IdFixo",
                table: "fnc_movimentacoes",
                column: "IdFixo",
                principalTable: "fnc_movimentacoes_fixas",
                principalColumn: "Id");
        }
    }
}
