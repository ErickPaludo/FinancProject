using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Financ.Infra.Data.Migrations
{
    /// <inheritdoc />
    public partial class inicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "fnc_contas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Titulo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    TipoConta = table.Column<int>(type: "int", nullable: false),
                    Saldo = table.Column<decimal>(type: "Decimal(18,2)", nullable: false),
                    Cor = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DthrReg = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_fnc_contas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "fnc_usuarios",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PrimeiroNome = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SegundoNome = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Salt = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HashPass = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_fnc_usuarios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "fnc_categorias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdConta = table.Column<int>(type: "int", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Cor = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_fnc_categorias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_fnc_categorias_fnc_contas_IdConta",
                        column: x => x.IdConta,
                        principalTable: "fnc_contas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "fnc_autenticacao",
                columns: table => new
                {
                    IdSession = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IdUsuario = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RefreshToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExpirationRefresh = table.Column<long>(type: "bigint", nullable: true),
                    Revoke = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_fnc_autenticacao", x => x.IdSession);
                    table.ForeignKey(
                        name: "FK_fnc_autenticacao_fnc_usuarios_IdUsuario",
                        column: x => x.IdUsuario,
                        principalTable: "fnc_usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "fnc_contas_usuarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdConta = table.Column<int>(type: "int", nullable: false),
                    IdUsuario = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Acesso = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Expiracao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DthrReg = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_fnc_contas_usuarios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_fnc_contas_usuarios_fnc_contas_IdConta",
                        column: x => x.IdConta,
                        principalTable: "fnc_contas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_fnc_contas_usuarios_fnc_usuarios_IdUsuario",
                        column: x => x.IdUsuario,
                        principalTable: "fnc_usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "fnc_convites",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdUsuarioRemetente = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IdUsuarioDestinatario = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IdConta = table.Column<int>(type: "int", nullable: false),
                    Acesso = table.Column<int>(type: "int", nullable: false),
                    Aceito = table.Column<bool>(type: "bit", nullable: true),
                    ExpiracaoContaUsuario = table.Column<int>(type: "int", nullable: true),
                    DataEnvio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Expiracao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Observacao = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_fnc_convites", x => x.Id);
                    table.ForeignKey(
                        name: "FK_fnc_convites_fnc_contas_IdConta",
                        column: x => x.IdConta,
                        principalTable: "fnc_contas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_fnc_convites_fnc_usuarios_IdUsuarioDestinatario",
                        column: x => x.IdUsuarioDestinatario,
                        principalTable: "fnc_usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_fnc_convites_fnc_usuarios_IdUsuarioRemetente",
                        column: x => x.IdUsuarioRemetente,
                        principalTable: "fnc_usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "fnc_movimentacoes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Tipo = table.Column<int>(type: "int", nullable: false),
                    IdConta = table.Column<int>(type: "int", nullable: false),
                    IdUsuarioCriador = table.Column<int>(type: "int", nullable: false),
                    IdUsuarioExecutor = table.Column<int>(type: "int", nullable: true),
                    IdCategoria = table.Column<int>(type: "int", nullable: true),
                    IdFixo = table.Column<int>(type: "int", nullable: false),
                    Valor = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Titulo = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Observacao = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    DthrReg = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DthrMovimentacao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DthrConclusao = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_fnc_movimentacoes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_fnc_movimentacoes_fnc_categorias_IdCategoria",
                        column: x => x.IdCategoria,
                        principalTable: "fnc_categorias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_fnc_movimentacoes_fnc_contas_IdConta",
                        column: x => x.IdConta,
                        principalTable: "fnc_contas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_fnc_movimentacoes_fnc_contas_usuarios_IdUsuarioCriador",
                        column: x => x.IdUsuarioCriador,
                        principalTable: "fnc_contas_usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_fnc_movimentacoes_fnc_contas_usuarios_IdUsuarioExecutor",
                        column: x => x.IdUsuarioExecutor,
                        principalTable: "fnc_contas_usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_fnc_autenticacao_IdUsuario",
                table: "fnc_autenticacao",
                column: "IdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_fnc_categorias_IdConta",
                table: "fnc_categorias",
                column: "IdConta");

            migrationBuilder.CreateIndex(
                name: "IX_fnc_contas_usuarios_IdConta",
                table: "fnc_contas_usuarios",
                column: "IdConta");

            migrationBuilder.CreateIndex(
                name: "IX_fnc_contas_usuarios_IdUsuario",
                table: "fnc_contas_usuarios",
                column: "IdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_fnc_convites_IdConta",
                table: "fnc_convites",
                column: "IdConta");

            migrationBuilder.CreateIndex(
                name: "IX_fnc_convites_IdUsuarioDestinatario",
                table: "fnc_convites",
                column: "IdUsuarioDestinatario");

            migrationBuilder.CreateIndex(
                name: "IX_fnc_convites_IdUsuarioRemetente",
                table: "fnc_convites",
                column: "IdUsuarioRemetente");

            migrationBuilder.CreateIndex(
                name: "IX_fnc_movimentacoes_IdCategoria",
                table: "fnc_movimentacoes",
                column: "IdCategoria");

            migrationBuilder.CreateIndex(
                name: "IX_fnc_movimentacoes_IdConta",
                table: "fnc_movimentacoes",
                column: "IdConta");

            migrationBuilder.CreateIndex(
                name: "IX_fnc_movimentacoes_IdUsuarioCriador",
                table: "fnc_movimentacoes",
                column: "IdUsuarioCriador");

            migrationBuilder.CreateIndex(
                name: "IX_fnc_movimentacoes_IdUsuarioExecutor",
                table: "fnc_movimentacoes",
                column: "IdUsuarioExecutor");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "fnc_autenticacao");

            migrationBuilder.DropTable(
                name: "fnc_convites");

            migrationBuilder.DropTable(
                name: "fnc_movimentacoes");

            migrationBuilder.DropTable(
                name: "fnc_categorias");

            migrationBuilder.DropTable(
                name: "fnc_contas_usuarios");

            migrationBuilder.DropTable(
                name: "fnc_contas");

            migrationBuilder.DropTable(
                name: "fnc_usuarios");
        }
    }
}
