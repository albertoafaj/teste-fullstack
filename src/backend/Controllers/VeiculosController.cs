using Microsoft.AspNetCore.Mvc;
using Parking.Api.Dtos;
using Parking.Api.Models;
using Parking.Api.Services.Interfaces;

namespace Parking.Api.Controllers
{
    /// <summary>
    /// Controlador responsável pelo gerenciamento de veículos.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class VeiculosController : ControllerBase
    {
        IVeiculosService _veiculosService;
        public VeiculosController(IVeiculosService veiculosService)
        {
            _veiculosService = veiculosService;
        }

        /// <summary>
        /// Lista veículos cadastrados, podendo filtrar por cliente.
        /// </summary>
        /// <param name="clienteId">Identificador único do cliente (opcional).</param>
        /// <returns>Lista de veículos.</returns>
        /// <response code="200">Retorna a lista de veículos.</response>
        [HttpGet]
        public async Task<IActionResult> List([FromQuery] Guid? clienteId = null)
        {
            List<Veiculo> response = await _veiculosService.Listar(clienteId);
            return Ok(response);
        }

        /// <summary>
        /// Cadastra um novo veículo.
        /// </summary>
        /// <param name="dto">Dados do veículo a ser cadastrado.</param>
        /// <returns>O veículo criado.</returns>
        /// <response code="201">Retorna o veículo criado.</response>
        /// <response code="400">Se os dados enviados forem inválidos.</response>
        /// /// <response code="404">Se o cliente não for encontrado.</response>
        /// <response code="409">Se placa já está associada a outro cliente cuja {dto.DataVigencia == null}</response>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] VeiculoDto dto)
        {
            Veiculo veiculo = await _veiculosService.Criar(dto);
            return CreatedAtAction(nameof(GetById), new { id = veiculo.Id }, veiculo);
        }

        /// <summary>
        /// Obtém um veículo pelo identificador.
        /// </summary>
        /// <param name="id">Identificador único do veículo.</param>
        /// <returns>O veículo encontrado.</returns>
        /// <response code="200">Retorna o veículo encontrado.</response>
        /// <response code="404">Se o veículo não for encontrado.</response>
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            Veiculo veiculo = await _veiculosService.GetById(id);
            return Ok(veiculo);
        }

        /// <summary>
        /// Atualiza os dados de um veículo existente.
        /// </summary>
        /// <param name="id">Identificador único do veículo.</param>
        /// <param name="dto">Novos dados do veículo.</param>
        /// <returns>O veículo atualizado.</returns>
        /// <response code="200">Retorna o veículo atualizado.</response>
        /// <response code="400">Se os dados enviados forem inválidos.</response>
        /// <response code="404">Se o veículo não for encontrado.</response>
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] VeiculoDto dto)
        {
            Veiculo? veiculo = await _veiculosService.Atualizar(id, dto);
            return Ok(veiculo);
        }

        /// <summary>
        /// Remove um veículo.
        /// </summary>
        /// <param name="id">Identificador único do veículo.</param>
        /// <returns>Resposta sem conteúdo em caso de sucesso.</returns>
        /// <response code="204">Veículo removido com sucesso.</response>
        /// <response code="404">Se o veículo não for encontrado.</response>
        /// <response code="409">Se tentar ataulizar a placa}</response>
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _veiculosService.Remover(id);
            return NoContent();
        }
    }
}
