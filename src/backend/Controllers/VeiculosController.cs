using Microsoft.AspNetCore.Mvc;
using Parking.Api.Dtos;
using Parking.Api.Models;
using Parking.Api.Services.Interfaces;

namespace Parking.Api.Controllers
{
    /// <summary>
    /// Controlador respons�vel pelo gerenciamento de ve�culos.
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
        /// Lista ve�culos cadastrados, podendo filtrar por cliente.
        /// </summary>
        /// <param name="clienteId">Identificador �nico do cliente (opcional).</param>
        /// <returns>Lista de ve�culos.</returns>
        /// <response code="200">Retorna a lista de ve�culos.</response>
        [HttpGet]
        public async Task<IActionResult> List([FromQuery] Guid? clienteId = null)
        {
            List<Veiculo> response = await _veiculosService.Listar(clienteId);
            return Ok(response);
        }

        /// <summary>
        /// Cadastra um novo ve�culo.
        /// </summary>
        /// <param name="dto">Dados do ve�culo a ser cadastrado.</param>
        /// <returns>O ve�culo criado.</returns>
        /// <response code="201">Retorna o ve�culo criado.</response>
        /// <response code="400">Se os dados enviados forem inv�lidos.</response>
        /// /// <response code="404">Se o cliente n�o for encontrado.</response>
        /// <response code="409">Se placa j� est� associada a outro cliente cuja {dto.DataVigencia == null}</response>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] VeiculoDto dto)
        {
            Veiculo veiculo = await _veiculosService.Criar(dto);
            return CreatedAtAction(nameof(GetById), new { id = veiculo.Id }, veiculo);
        }

        /// <summary>
        /// Obt�m um ve�culo pelo identificador.
        /// </summary>
        /// <param name="id">Identificador �nico do ve�culo.</param>
        /// <returns>O ve�culo encontrado.</returns>
        /// <response code="200">Retorna o ve�culo encontrado.</response>
        /// <response code="404">Se o ve�culo n�o for encontrado.</response>
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            Veiculo veiculo = await _veiculosService.GetById(id);
            return Ok(veiculo);
        }

        /// <summary>
        /// Atualiza os dados de um ve�culo existente.
        /// </summary>
        /// <param name="id">Identificador �nico do ve�culo.</param>
        /// <param name="dto">Novos dados do ve�culo.</param>
        /// <returns>O ve�culo atualizado.</returns>
        /// <response code="200">Retorna o ve�culo atualizado.</response>
        /// <response code="400">Se os dados enviados forem inv�lidos.</response>
        /// <response code="404">Se o ve�culo n�o for encontrado.</response>
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] VeiculoDto dto)
        {
            Veiculo? veiculo = await _veiculosService.Atualizar(id, dto);
            return Ok(veiculo);
        }

        /// <summary>
        /// Remove um ve�culo.
        /// </summary>
        /// <param name="id">Identificador �nico do ve�culo.</param>
        /// <returns>Resposta sem conte�do em caso de sucesso.</returns>
        /// <response code="204">Ve�culo removido com sucesso.</response>
        /// <response code="404">Se o ve�culo n�o for encontrado.</response>
        /// <response code="409">Se tentar ataulizar a placa}</response>
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _veiculosService.Remover(id);
            return NoContent();
        }
    }
}
