using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Financ.Infra.Data.Migrations
{
    /// <inheritdoc />
    public partial class camposusuariosfk5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_fnc_movimentacoes_fnc_contas_usuarios_ContaUsuarioCriadorId",
                table: "fnc_movimentacoes");

            migrationBuilder.DropIndex(
                name: "IX_fnc_movimentacoes_ContaUsuarioCriadorId",
                table: "fnc_movimentacoes");

            migrationBuilder.DropColumn(
                name: "ContaUsuarioCriadorId",
                table: "fnc_movimentacoes");

            migrationBuilder.CreateIndex(
                name: "IX_fnc_movimentacoes_IdUsuarioCriador",
                table: "fnc_movimentacoes",
                column: "IdUsuarioCriador");

            migrationBuilder.AddForeignKey(
                name: "FK_fnc_movimentacoes_fnc_contas_usuarios_IdUsuarioCriador",
                table: "fnc_movimentacoes",
                column: "IdUsuarioCriador",
                principalTable: "fnc_contas_usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_fnc_movimentacoes_fnc_contas_usuarios_IdUsuarioCriador",
                table: "fnc_movimentacoes");

            migrationBuilder.DropIndex(
                name: "IX_fnc_movimentacoes_IdUsuarioCriador",
                table: "fnc_movimentacoes");

            migrationBuilder.AddColumn<int>(
                name: "ContaUsuarioCriadorId",
                table: "fnc_movimentacoes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_fnc_movimentacoes_ContaUsuarioCriadorId",
                table: "fnc_movimentacoes",
                column: "ContaUsuarioCriadorId");

            migrationBuilder.AddForeignKey(
                name: "FK_fnc_movimentacoes_fnc_contas_usuarios_ContaUsuarioCriadorId",
                table: "fnc_movimentacoes",
                column: "ContaUsuarioCriadorId",
                principalTable: "fnc_contas_usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
