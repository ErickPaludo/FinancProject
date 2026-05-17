using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Financ.Infra.Data.Migrations
{
    /// <inheritdoc />
    public partial class TabelaDeLinhasMovCategorias : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "fnc_movimentacao_categorias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdMovimentacao = table.Column<int>(type: "int", nullable: false),
                    IdCategoria = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_fnc_movimentacao_categorias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_fnc_movimentacao_categorias_fnc_categorias_IdCategoria",
                        column: x => x.IdCategoria,
                        principalTable: "fnc_categorias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_fnc_movimentacao_categorias_fnc_movimentacoes_IdMovimentacao",
                        column: x => x.IdMovimentacao,
                        principalTable: "fnc_movimentacoes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_fnc_movimentacao_categorias_IdCategoria",
                table: "fnc_movimentacao_categorias",
                column: "IdCategoria");

            migrationBuilder.CreateIndex(
                name: "IX_fnc_movimentacao_categorias_IdMovimentacao",
                table: "fnc_movimentacao_categorias",
                column: "IdMovimentacao");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "fnc_movimentacao_categorias");
        }
    }
}
