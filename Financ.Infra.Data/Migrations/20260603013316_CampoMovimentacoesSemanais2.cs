using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Financ.Infra.Data.Migrations
{
    /// <inheritdoc />
    public partial class CampoMovimentacoesSemanais2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MovimentacaoFixaId",
                table: "fnc_movimentacoes_fixas_semanal",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<DateOnly>(
                name: "DataOcorrencia",
                table: "fnc_movimentacoes_fixas",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.CreateIndex(
                name: "IX_fnc_movimentacoes_fixas_semanal_MovimentacaoFixaId",
                table: "fnc_movimentacoes_fixas_semanal",
                column: "MovimentacaoFixaId");

            migrationBuilder.AddForeignKey(
                name: "FK_fnc_movimentacoes_fixas_semanal_fnc_movimentacoes_fixas_MovimentacaoFixaId",
                table: "fnc_movimentacoes_fixas_semanal",
                column: "MovimentacaoFixaId",
                principalTable: "fnc_movimentacoes_fixas",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_fnc_movimentacoes_fixas_semanal_fnc_movimentacoes_fixas_MovimentacaoFixaId",
                table: "fnc_movimentacoes_fixas_semanal");

            migrationBuilder.DropIndex(
                name: "IX_fnc_movimentacoes_fixas_semanal_MovimentacaoFixaId",
                table: "fnc_movimentacoes_fixas_semanal");

            migrationBuilder.DropColumn(
                name: "MovimentacaoFixaId",
                table: "fnc_movimentacoes_fixas_semanal");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "DataOcorrencia",
                table: "fnc_movimentacoes_fixas",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1),
                oldClrType: typeof(DateOnly),
                oldType: "date",
                oldNullable: true);
        }
    }
}
