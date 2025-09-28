using Microsoft.AspNetCore.Mvc;

using Parking.Api.Dtos;
using Parking.Api.Models;
using Parking.Api.Services.Interfaces;

namespace Parking.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FaturasController(IFaturamentoService faturamentoService) : ControllerBase
    {
        [HttpPost("gerar")]
        public async Task<IActionResult> Gerar([FromBody] GerarFaturaRequest req, CancellationToken ct)
        {
            List<Fatura> criadas = await faturamentoService.GerarAsync(req.Competencia, ct);
            return Ok(new { criadas = criadas.Count });
        }

        [HttpGet]
        public async Task<IActionResult> List([FromQuery] string? competencia = null)
        {
            List<FaturaDto> faturas = await faturamentoService.Listar(competencia);
            return Ok(faturas);
        }

        [HttpGet("{id:guid}/placas")]
        public async Task<IActionResult> Placas(Guid id)
        {
            List<string> placas = await faturamentoService.ListarPlacasPorFaturaId(id);
            return Ok(placas);
        }
    }
}
