using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Parking.Api.Migrations
{
    /// <inheritdoc />
    public partial class VeiculoIndexPlacaVigencia : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_veiculo_placa",
                schema: "public",
                table: "veiculo");

            migrationBuilder.CreateIndex(
                name: "IX_veiculo_placa_data_vigencia",
                schema: "public",
                table: "veiculo",
                columns: new[] { "placa", "data_vigencia" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_veiculo_placa_data_vigencia",
                schema: "public",
                table: "veiculo");

            migrationBuilder.CreateIndex(
                name: "IX_veiculo_placa",
                schema: "public",
                table: "veiculo",
                column: "placa",
                unique: true);
        }
    }
}
