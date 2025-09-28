
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Parking.Api.Data;
using Parking.Api.Dtos;
using Parking.Api.Models;
using Parking.Api.Services;

namespace Parking.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VeiculosController : ControllerBase
    {
        IVeiculosService _veiculosService;
        public VeiculosController(IVeiculosService veiculosService)
        {
            _veiculosService = veiculosService;
        }

        [HttpGet]
        public async Task<IActionResult> List([FromQuery] Guid? clienteId = null)
        {
            List<Veiculo> response = await _veiculosService.Listar(clienteId);
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] VeiculoDto dto)
        {
            Veiculo veiculo = await _veiculosService.Criar(dto);
            return CreatedAtAction(nameof(GetById), new { id = veiculo.Id }, veiculo);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            Veiculo veiculo = await _veiculosService.GetById(id);
            return Ok(veiculo);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] VeiculoDto dto)
        {
            Veiculo? veiculo = await _veiculosService.Atualizar(id, dto);
            return Ok(veiculo);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _veiculosService.Remover(id);
            return NoContent();
        }
    }
}
