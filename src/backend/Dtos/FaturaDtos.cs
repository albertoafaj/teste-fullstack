
namespace Parking.Api.Dtos
{
    public record GerarFaturaRequest(string Competencia);
    public record FaturaDto(Guid Id, string Competencia, Guid ClienteId, decimal Valor, DateTime CriadaEm, int qtdVeiculos);
}
