using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FaturamentoService.Migrations
{
    /// <inheritdoc />
    public partial class ModificacaoTabelasDeFaturamento : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NotaFiscalItens_NotasFiscais_NotaFiscalId",
                table: "NotaFiscalItens");

            migrationBuilder.AddColumn<DateTime>(
                name: "DataEmissao",
                table: "NotasFiscais",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<Guid>(
                name: "NotaFiscalId",
                table: "NotaFiscalItens",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_NotaFiscalItens_NotasFiscais_NotaFiscalId",
                table: "NotaFiscalItens",
                column: "NotaFiscalId",
                principalTable: "NotasFiscais",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NotaFiscalItens_NotasFiscais_NotaFiscalId",
                table: "NotaFiscalItens");

            migrationBuilder.DropColumn(
                name: "DataEmissao",
                table: "NotasFiscais");

            migrationBuilder.AlterColumn<Guid>(
                name: "NotaFiscalId",
                table: "NotaFiscalItens",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddForeignKey(
                name: "FK_NotaFiscalItens_NotasFiscais_NotaFiscalId",
                table: "NotaFiscalItens",
                column: "NotaFiscalId",
                principalTable: "NotasFiscais",
                principalColumn: "Id");
        }
    }
}
