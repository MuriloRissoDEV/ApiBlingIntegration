using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ApiBling.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class TabelaDimensaoSituacaoNFSe : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SituacoesNotasFiscaisServico",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SituacoesNotasFiscaisServico", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "SituacoesNotasFiscaisServico",
                columns: new[] { "Id", "Nome" },
                values: new object[,]
                {
                    { 0, "Pendente" },
                    { 1, "Emitida" },
                    { 2, "Disponível para consulta" },
                    { 3, "Cancelada" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_NotasFiscaisServico_Situacao",
                table: "NotasFiscaisServico",
                column: "Situacao");

            migrationBuilder.AddForeignKey(
                name: "FK_NotasFiscaisServico_SituacoesNotasFiscaisServico_Situacao",
                table: "NotasFiscaisServico",
                column: "Situacao",
                principalTable: "SituacoesNotasFiscaisServico",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NotasFiscaisServico_SituacoesNotasFiscaisServico_Situacao",
                table: "NotasFiscaisServico");

            migrationBuilder.DropTable(
                name: "SituacoesNotasFiscaisServico");

            migrationBuilder.DropIndex(
                name: "IX_NotasFiscaisServico_Situacao",
                table: "NotasFiscaisServico");
        }
    }
}
