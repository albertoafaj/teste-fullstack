using Microsoft.AspNetCore.Mvc;
using Parking.Api.Dtos;
using Parking.Api.Exceptions;
using Parking.Api.Models;
using System.Linq.Expressions;

namespace Parking.Api.Services.Interfaces;

/// <summary>
/// Serviço responsável por gerar e listar faturas de clientes mensalistas.
/// </summary>
public interface IFaturamentoService
{
    /// <summary>
    /// Gera faturas para todos os clientes mensalistas em uma determinada competência.
    /// </summary>
    /// <param name="competencia">Competência no formato yyyy-MM (ex.: 2025-09).</param>
    /// <param name="ct">Token de cancelamento opcional.</param>
    /// <returns>Lista de faturas geradas.</returns>
    /// <exception cref="BadRequestException">Se a competência estiver em formato inválido.</exception>
    Task<List<Fatura>> GerarAsync(string competencia, CancellationToken ct = default);

    /// <summary>
    /// Lista faturas com filtro opcional por competência.
    /// </summary>
    /// <param name="competencia">Competência no formato yyyy-MM.</param>
    /// <returns>Lista de faturas DTO.</returns>
    Task<List<FaturaDto>> Listar([FromQuery] string? competencia = null);

    /// <summary>
    /// Conta a quantidade de veículos associados a faturas com base no predicado.
    /// </summary>
    /// <param name="predicate">Predicado para filtro.</param>
    /// <returns>Número de veículos associados.</returns>
    Task<int> ObterFaturasVeiculos(Expression<Func<FaturaVeiculo, bool>> predicate);

    /// <summary>
    /// Lista as placas de veículos associadas a uma fatura.
    /// </summary>
    /// <param name="id">Identificador da fatura.</param>
    /// <returns>Lista de placas.</returns>
    Task<List<string>> ListarPlacasPorFaturaId(Guid id);
}
