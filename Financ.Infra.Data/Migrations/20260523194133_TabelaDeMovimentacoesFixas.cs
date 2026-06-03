using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Financ.Infra.Data.Migrations
{
    /// <inheritdoc />
    public partial class TabelaDeMovimentacoesFixas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "fnc_movimentacoes_fixas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdMovimentacao = table.Column<int>(type: "int", nullable: false),
                    Tipo = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    DataInicio = table.Column<DateOnly>(type: "date", nullable: false),
                    DataFim = table.Column<DateOnly>(type: "date", nullable: false),
                    DataOcorrencia = table.Column<DateOnly>(type: "date", nullable: false),
                    Dthr = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_fnc_movimentacoes_fixas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_fnc_movimentacoes_fixas_fnc_movimentacoes_IdMovimentacao",
                        column: x => x.IdMovimentacao,
                        principalTable: "fnc_movimentacoes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_fnc_movimentacoes_fixas_IdMovimentacao",
                table: "fnc_movimentacoes_fixas",
                column: "IdMovimentacao");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "fnc_movimentacoes_fixas");
        }
    }
}
