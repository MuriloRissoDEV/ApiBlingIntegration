using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApiBling.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTabelasNotasFiscais : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NotasFiscais",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BlingId = table.Column<long>(type: "bigint", nullable: false),
                    Tipo = table.Column<int>(type: "int", nullable: false),
                    Situacao = table.Column<int>(type: "int", nullable: false),
                    Numero = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DataEmissao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataOperacao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ChaveAcesso = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LojaId = table.Column<long>(type: "bigint", nullable: false),
                    ContatoNome = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContatoDocumento = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Serie = table.Column<int>(type: "int", nullable: true),
                    ValorNota = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    ValorFrete = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    Finalidade = table.Column<int>(type: "int", nullable: true),
                    TipoNota = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LinkDanfe = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LinkPDF = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NumeroPedidoLoja = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DetalhesSincronizados = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotasFiscais", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NotaFiscalItens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NotaFiscalId = table.Column<int>(type: "int", nullable: false),
                    Codigo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Unidade = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Quantidade = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    Valor = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    ValorTotal = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    Tipo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ClassificacaoFiscal = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Cest = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Cfop = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotaFiscalItens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NotaFiscalItens_NotasFiscais_NotaFiscalId",
                        column: x => x.NotaFiscalId,
                        principalTable: "NotasFiscais",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NotaFiscalItens_NotaFiscalId",
                table: "NotaFiscalItens",
                column: "NotaFiscalId");

            migrationBuilder.CreateIndex(
                name: "IX_NotasFiscais_BlingId",
                table: "NotasFiscais",
                column: "BlingId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NotaFiscalItens");

            migrationBuilder.DropTable(
                name: "NotasFiscais");
        }
    }
}
