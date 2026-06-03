using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Financ.Infra.Data.Migrations
{
    /// <inheritdoc />
    public partial class CampoMovimentacaoDeleteCascade2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_fnc_movimentacoes_fnc_movimentacoes_fixas_IdFixo",
                table: "fnc_movimentacoes");

            migrationBuilder.DropForeignKey(
                name: "FK_fnc_movimentacoes_fixas_fnc_movimentacoes_IdMovimentacao",
                table: "fnc_movimentacoes_fixas");

            migrationBuilder.AddForeignKey(
                name: "FK_fnc_movimentacoes_fnc_movimentacoes_fixas_IdFixo",
                table: "fnc_movimentacoes",
                column: "IdFixo",
                principalTable: "fnc_movimentacoes_fixas",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_fnc_movimentacoes_fixas_fnc_movimentacoes_IdMovimentacao",
                table: "fnc_movimentacoes_fixas",
                column: "IdMovimentacao",
                principalTable: "fnc_movimentacoes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_fnc_movimentacoes_fnc_movimentacoes_fixas_IdFixo",
                table: "fnc_movimentacoes");

            migrationBuilder.DropForeignKey(
                name: "FK_fnc_movimentacoes_fixas_fnc_movimentacoes_IdMovimentacao",
                table: "fnc_movimentacoes_fixas");

            migrationBuilder.AddForeignKey(
                name: "FK_fnc_movimentacoes_fnc_movimentacoes_fixas_IdFixo",
                table: "fnc_movimentacoes",
                column: "IdFixo",
                principalTable: "fnc_movimentacoes_fixas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_fnc_movimentacoes_fixas_fnc_movimentacoes_IdMovimentacao",
                table: "fnc_movimentacoes_fixas",
                column: "IdMovimentacao",
                principalTable: "fnc_movimentacoes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
