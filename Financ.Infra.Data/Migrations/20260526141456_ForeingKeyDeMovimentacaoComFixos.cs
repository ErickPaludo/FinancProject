using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Financ.Infra.Data.Migrations
{
    /// <inheritdoc />
    public partial class ForeingKeyDeMovimentacaoComFixos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "IdFixo",
                table: "fnc_movimentacoes",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_fnc_movimentacoes_IdFixo",
                table: "fnc_movimentacoes",
                column: "IdFixo");

            migrationBuilder.AddForeignKey(
                name: "FK_fnc_movimentacoes_fnc_movimentacoes_fixas_IdFixo",
                table: "fnc_movimentacoes",
                column: "IdFixo",
                principalTable: "fnc_movimentacoes_fixas",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_fnc_movimentacoes_fnc_movimentacoes_fixas_IdFixo",
                table: "fnc_movimentacoes");

            migrationBuilder.DropIndex(
                name: "IX_fnc_movimentacoes_IdFixo",
                table: "fnc_movimentacoes");

            migrationBuilder.AlterColumn<int>(
                name: "IdFixo",
                table: "fnc_movimentacoes",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
