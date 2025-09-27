using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Parking.Api.Migrations;

/// <inheritdoc />
public partial class SeedClientesVeiculos : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // Seed Clientes
        migrationBuilder.InsertData(
            table: "cliente",
            schema: "public",
            columns: new[] { "id", "nome", "telefone", "endereco", "mensalista", "valor_mensalidade", "data_inclusao" },
            values: new object[,]
            {
                { Guid.Parse("11111111-1111-1111-1111-111111111111"), "João Souza", "31999990001", "Rua A, 123", true, 189.90m, DateTime.UtcNow },
                { Guid.Parse("22222222-2222-2222-2222-222222222222"), "Maria Lima", "31988880002", "Av. B, 456", false, null, DateTime.UtcNow },
                { Guid.Parse("33333333-3333-3333-3333-333333333333"), "Carlos Silva", "31977770003", "Rua C, 789", true, 159.90m, DateTime.UtcNow },
                { Guid.Parse("44444444-4444-4444-4444-444444444444"), "Ana Paula", "31966660004", "Av. D, 101", false, null, DateTime.UtcNow },
                { Guid.Parse("55555555-5555-5555-5555-555555555555"), "Beatriz Melo", "31955550005", "Rua E, 202", true, 209.90m, DateTime.UtcNow }
            });

        // Seed Veículos
        migrationBuilder.InsertData(
            table: "veiculo",
            schema: "public",
            columns: new[] { "id", "placa", "modelo", "ano", "cliente_id", "data_inclusao" },
            values: new object[,]
            {
                { Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa1"), "BRA1A23", "Gol", 2019, Guid.Parse("11111111-1111-1111-1111-111111111111"), new DateTime(2025, 7, 10).ToUniversalTime()},
                { Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa2"), "RCH2B45", "Onix", 2020, Guid.Parse("22222222-2222-2222-2222-222222222222"), new DateTime(2025, 7, 15).ToUniversalTime() },
                { Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa3"), "ABC1D23", "HB20", 2018, Guid.Parse("11111111-1111-1111-1111-111111111111"), new DateTime(2025, 8, 1).ToUniversalTime() },
                { Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa4"), "QWE1Z89", "Argo", 2021, Guid.Parse("33333333-3333-3333-3333-333333333333"), new DateTime(2025, 7, 20).ToUniversalTime() },
                { Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa5"), "JKL2M34", "Fox", 2017, Guid.Parse("33333333-3333-3333-3333-333333333333"), new DateTime(2025, 8, 5).ToUniversalTime() },
                { Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa6"), "ZTB3N56", "Civic", 2022, Guid.Parse("55555555-5555-5555-5555-555555555555"), new DateTime(2025, 7, 1).ToUniversalTime() },
                { Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa7"), "HGF4P77", "Corolla", 2022, Guid.Parse("55555555-5555-5555-5555-555555555555"), new DateTime(2025, 8, 20).ToUniversalTime() },
                { Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa8"), "AAA1A11", "Uno", 2015, Guid.Parse("22222222-2222-2222-2222-222222222222"), new DateTime(2025, 7, 1).ToUniversalTime() }
            });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DeleteData(
            table: "veiculo",
            keyColumn: "id",
            keyValues: new object[]
            {
                Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa1"),
                Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa2"),
                Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa3"),
                Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa4"),
                Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa5"),
                Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa6"),
                Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa7"),
                Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa8")
            });

        migrationBuilder.DeleteData(
            table: "cliente",
            keyColumn: "id",
            keyValues: new object[]
            {
                Guid.Parse("11111111-1111-1111-1111-111111111111"),
                Guid.Parse("22222222-2222-2222-2222-222222222222"),
                Guid.Parse("33333333-3333-3333-3333-333333333333"),
                Guid.Parse("44444444-4444-4444-4444-444444444444"),
                Guid.Parse("55555555-5555-5555-5555-555555555555")
            });
    }
}