using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApiBling.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoverTabelaSituacoesNFe : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SituacoesNotasFiscais");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SituacoesNotasFiscais",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    Cor = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IdHerdado = table.Column<long>(type: "bigint", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SituacoesNotasFiscais", x => x.Id);
                });
        }
    }
}
