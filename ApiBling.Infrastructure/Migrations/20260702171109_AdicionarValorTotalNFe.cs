using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApiBling.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarValorTotalNFe : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "ValorTotal",
                table: "notas_fiscais_gerais",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ValorTotal",
                table: "notas_fiscais_gerais");
        }
    }
}
