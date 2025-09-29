using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Parking.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueIndexClienteNomeTelefone : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_cliente_nome_telefone",
                schema: "public",
                table: "cliente");

            migrationBuilder.CreateIndex(
                name: "IX_cliente_nome_telefone_not_null",
                schema: "public",
                table: "cliente",
                columns: new[] { "nome", "telefone" },
                unique: true,
                filter: "\"telefone\" IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_cliente_nome_telefone_not_null",
                schema: "public",
                table: "cliente");

            migrationBuilder.CreateIndex(
                name: "IX_cliente_nome_telefone",
                schema: "public",
                table: "cliente",
                columns: new[] { "nome", "telefone" });
        }
    }
}
