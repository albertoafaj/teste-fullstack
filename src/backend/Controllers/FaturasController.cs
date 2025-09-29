using Microsoft.AspNetCore.Mvc;
using Parking.Api.Dtos;
using Parking.Api.Models;
using Parking.Api.Services.Interfaces;

namespace Parking.Api.Controllers;

/// <summary>
/// Controlador respons�vel pelo gerenciamento de faturas.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class FaturasController(IFaturamentoService faturamentoService) : ControllerBase
{
    /// <summary>
    /// Gera faturas para a compet�ncia informada.
    /// </summary>
    /// <param name="req">Requisi��o contendo a compet�ncia para gera��o da fatura.</param>
    /// <param name="ct">Token de cancelamento da opera��o ass�ncrona.</param>
    /// <returns>Quantidade de faturas criadas.</returns>
    /// <response code="200">Retorna a quantidade de faturas geradas.</response>
    /// <response code="400">Se a requisi��o estiver inv�lida.</response>
    [HttpPost("gerar")]
    public async Task<IActionResult> Gerar([FromBody] GerarFaturaRequest req, CancellationToken ct)
    {
        List<Fatura> criadas = await faturamentoService.GerarAsync(req.Competencia, ct);
        return Ok(new { criadas = criadas.Count });
    }

    /// <summary>
    /// Lista todas as faturas, podendo filtrar por compet�ncia.
    /// </summary>
    /// <param name="competencia">Compet�ncia no formato AAAA-MM (opcional).</param>
    /// <returns>Lista de faturas.</returns>
    /// <response code="200">Retorna a lista de faturas.</response>
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] string? competencia = null)
    {
        List<FaturaDto> faturas = await faturamentoService.Listar(competencia);
        return Ok(faturas);
    }

    /// <summary>
    /// Lista as placas de ve�culos associadas a uma fatura.
    /// </summary>
    /// <param name="id">Identificador �nico da fatura.</param>
    /// <returns>Lista de placas associadas � fatura.</returns>
    /// <response code="200">Retorna a lista de placas vinculadas � fatura.</response>
    /// <response code="404">Se a fatura n�o for encontrada.</response>
    [HttpGet("{id:guid}/placas")]
    public async Task<IActionResult> Placas(Guid id)
    {
        List<string> placas = await faturamentoService.ListarPlacasPorFaturaId(id);
        return Ok(placas);
    }
}
