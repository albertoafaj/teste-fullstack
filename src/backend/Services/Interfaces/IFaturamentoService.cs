using Microsoft.AspNetCore.Mvc;
using Parking.Api.Dtos;
using Parking.Api.Models;
using System.Linq.Expressions;

namespace Parking.Api.Services.Interfaces;

public interface IFaturamentoService
{
    Task<List<Fatura>> GerarAsync(string competencia, CancellationToken ct = default);
    Task<List<FaturaDto>> Listar([FromQuery] string? competencia = null);
    Task<int> ObterFaturasVeiculos(Expression<Func<FaturaVeiculo, bool>> predicate);
    Task<List<string>> ListarPlacasPorFaturaId(Guid id);
}
