using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApiBling.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AlterarIdNaturezaParaLong : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Remove a Chave Primária atual para destravar a coluna
            migrationBuilder.DropPrimaryKey(
                name: "PK_naturezas_operacoes",
                table: "naturezas_operacoes");

            // 2. Altera o tipo da coluna de int para long (bigint no SQL)
            migrationBuilder.AlterColumn<long>(
                name: "id",
                table: "naturezas_operacoes",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            // 3. Recria a Chave Primária
            migrationBuilder.AddPrimaryKey(
                name: "PK_naturezas_operacoes",
                table: "naturezas_operacoes",
                column: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "naturezas_operacoes",
                type: "int",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");
        }
    }
}
