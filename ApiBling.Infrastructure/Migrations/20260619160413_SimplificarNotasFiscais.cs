using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApiBling.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SimplificarNotasFiscais : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChaveAcesso",
                table: "NotasFiscais");

            migrationBuilder.DropColumn(
                name: "ContatoDocumento",
                table: "NotasFiscais");

            migrationBuilder.DropColumn(
                name: "ContatoNome",
                table: "NotasFiscais");

            migrationBuilder.DropColumn(
                name: "Finalidade",
                table: "NotasFiscais");

            migrationBuilder.DropColumn(
                name: "LinkDanfe",
                table: "NotasFiscais");

            migrationBuilder.DropColumn(
                name: "LinkPDF",
                table: "NotasFiscais");

            migrationBuilder.DropColumn(
                name: "NumeroPedidoLoja",
                table: "NotasFiscais");

            migrationBuilder.DropColumn(
                name: "Serie",
                table: "NotasFiscais");

            migrationBuilder.DropColumn(
                name: "Situacao",
                table: "NotasFiscais");

            migrationBuilder.DropColumn(
                name: "Tipo",
                table: "NotasFiscais");

            migrationBuilder.DropColumn(
                name: "Cest",
                table: "NotaFiscalItens");

            migrationBuilder.DropColumn(
                name: "Cfop",
                table: "NotaFiscalItens");

            migrationBuilder.DropColumn(
                name: "ClassificacaoFiscal",
                table: "NotaFiscalItens");

            migrationBuilder.DropColumn(
                name: "Codigo",
                table: "NotaFiscalItens");

            migrationBuilder.DropColumn(
                name: "Quantidade",
                table: "NotaFiscalItens");

            migrationBuilder.DropColumn(
                name: "Tipo",
                table: "NotaFiscalItens");

            migrationBuilder.DropColumn(
                name: "Valor",
                table: "NotaFiscalItens");

            migrationBuilder.DropColumn(
                name: "ValorTotal",
                table: "NotaFiscalItens");

            migrationBuilder.RenameColumn(
                name: "TipoNota",
                table: "NotasFiscais",
                newName: "ClienteNome");

            migrationBuilder.RenameColumn(
                name: "Unidade",
                table: "NotaFiscalItens",
                newName: "CodigoProduto");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DataEmissao",
                table: "NotasFiscais",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<long>(
                name: "NaturezaOperacaoId",
                table: "NotasFiscais",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<bool>(
                name: "OptanteSimplesNacional",
                table: "NotasFiscais",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NaturezaOperacaoId",
                table: "NotasFiscais");

            migrationBuilder.DropColumn(
                name: "OptanteSimplesNacional",
                table: "NotasFiscais");

            migrationBuilder.RenameColumn(
                name: "ClienteNome",
                table: "NotasFiscais",
                newName: "TipoNota");

            migrationBuilder.RenameColumn(
                name: "CodigoProduto",
                table: "NotaFiscalItens",
                newName: "Unidade");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DataEmissao",
                table: "NotasFiscais",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ChaveAcesso",
                table: "NotasFiscais",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ContatoDocumento",
                table: "NotasFiscais",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ContatoNome",
                table: "NotasFiscais",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Finalidade",
                table: "NotasFiscais",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LinkDanfe",
                table: "NotasFiscais",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LinkPDF",
                table: "NotasFiscais",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NumeroPedidoLoja",
                table: "NotasFiscais",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Serie",
                table: "NotasFiscais",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Situacao",
                table: "NotasFiscais",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Tipo",
                table: "NotasFiscais",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Cest",
                table: "NotaFiscalItens",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Cfop",
                table: "NotaFiscalItens",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ClassificacaoFiscal",
                table: "NotaFiscalItens",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Codigo",
                table: "NotaFiscalItens",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "Quantidade",
                table: "NotaFiscalItens",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Tipo",
                table: "NotaFiscalItens",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "Valor",
                table: "NotaFiscalItens",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ValorTotal",
                table: "NotaFiscalItens",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                defaultValue: 0m);
        }
    }
}
