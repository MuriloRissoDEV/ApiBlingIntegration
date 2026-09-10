using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApiBling.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarDetalhesNFe : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "DetalhesSincronizados",
                table: "notas_fiscais_gerais",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Finalidade",
                table: "notas_fiscais_gerais",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TipoNota",
                table: "notas_fiscais_gerais",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ValorFrete",
                table: "notas_fiscais_gerais",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "NotasFiscaisGeraisItens",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdNotaFiscalGeral = table.Column<long>(type: "bigint", nullable: false),
                    Codigo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Unidade = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Quantidade = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Valor = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ValorTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Tipo = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotasFiscaisGeraisItens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NotasFiscaisGeraisItens_notas_fiscais_gerais_IdNotaFiscalGeral",
                        column: x => x.IdNotaFiscalGeral,
                        principalTable: "notas_fiscais_gerais",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NotasFiscaisGeraisItens_IdNotaFiscalGeral",
                table: "NotasFiscaisGeraisItens",
                column: "IdNotaFiscalGeral");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NotasFiscaisGeraisItens");

            migrationBuilder.DropColumn(
                name: "DetalhesSincronizados",
                table: "notas_fiscais_gerais");

            migrationBuilder.DropColumn(
                name: "Finalidade",
                table: "notas_fiscais_gerais");

            migrationBuilder.DropColumn(
                name: "TipoNota",
                table: "notas_fiscais_gerais");

            migrationBuilder.DropColumn(
                name: "ValorFrete",
                table: "notas_fiscais_gerais");
        }
    }
}
