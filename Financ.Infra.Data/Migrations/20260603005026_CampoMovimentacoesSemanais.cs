using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Financ.Infra.Data.Migrations
{
    /// <inheritdoc />
    public partial class CampoMovimentacoesSemanais : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "fnc_movimentacoes_fixas_semanal",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdFixo = table.Column<int>(type: "int", nullable: false),
                    DiaSemana = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_fnc_movimentacoes_fixas_semanal", x => x.Id);
                    table.ForeignKey(
                        name: "FK_fnc_movimentacoes_fixas_semanal_fnc_movimentacoes_fixas_IdFixo",
                        column: x => x.IdFixo,
                        principalTable: "fnc_movimentacoes_fixas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_fnc_movimentacoes_fixas_semanal_IdFixo",
                table: "fnc_movimentacoes_fixas_semanal",
                column: "IdFixo");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "fnc_movimentacoes_fixas_semanal");
        }
    }
}
