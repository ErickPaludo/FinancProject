using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Financ.Infra.Data.Migrations
{
    /// <inheritdoc />
    public partial class CampoContaMovimmentacaoFixa : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IdConta",
                table: "fnc_movimentacoes_fixas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_fnc_movimentacoes_fixas_IdConta",
                table: "fnc_movimentacoes_fixas",
                column: "IdConta");

            migrationBuilder.AddForeignKey(
                name: "FK_fnc_movimentacoes_fixas_fnc_contas_IdConta",
                table: "fnc_movimentacoes_fixas",
                column: "IdConta",
                principalTable: "fnc_contas",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_fnc_movimentacoes_fixas_fnc_contas_IdConta",
                table: "fnc_movimentacoes_fixas");

            migrationBuilder.DropIndex(
                name: "IX_fnc_movimentacoes_fixas_IdConta",
                table: "fnc_movimentacoes_fixas");

            migrationBuilder.DropColumn(
                name: "IdConta",
                table: "fnc_movimentacoes_fixas");
        }
    }
}
