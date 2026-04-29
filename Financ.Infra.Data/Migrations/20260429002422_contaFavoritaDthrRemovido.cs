using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Financ.Infra.Data.Migrations
{
    /// <inheritdoc />
    public partial class contaFavoritaDthrRemovido : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Dthr",
                table: "fnc_contas_usuarios");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "Dthr",
                table: "fnc_contas_usuarios",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
