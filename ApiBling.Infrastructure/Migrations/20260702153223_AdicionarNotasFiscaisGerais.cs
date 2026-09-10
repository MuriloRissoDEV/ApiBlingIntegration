using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApiBling.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarNotasFiscaisGerais : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "notas_fiscais_gerais",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false),
                    tipo = table.Column<int>(type: "int", maxLength: 10, nullable: false),
                    situacao = table.Column<int>(type: "int", maxLength: 30, nullable: false),
                    numero = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    data_emissao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    data_operacao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    chave_acesso = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    natureza_operacao = table.Column<long>(type: "bigint", maxLength: 120, nullable: true),
                    Id_Loja = table.Column<long>(type: "bigint", maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_notas_fiscais_gerais", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "notas_fiscais_gerais");
        }
    }
}
