
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
    public class ClientesController : ControllerBase
    {
        IClienteService _clienteService;
        private readonly AppDbContext _db;

        public ClientesController(AppDbContext db, IClienteService clienteService)
        {
            _db = db;
            _clienteService = clienteService;
        }

        [HttpGet]
        public async Task<IActionResult> List([FromQuery] int pagina = 1, [FromQuery] int tamanho = 10, [FromQuery] string? filtro = null, [FromQuery] string mensalista = "all")
        {
            (int total, List<Cliente> itens) result;
            try
            {
                result = await _clienteService.Listar(pagina, tamanho, filtro, mensalista);
                return Ok(new { result.total, result.itens });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.InnerException == null ? "Erro desconhecido. Contate o administrador." : ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ClienteDto dto)
        {
            try
            {
                Cliente cliente = await _clienteService.Criar(dto);
                return CreatedAtAction(nameof(GetById), new { id = cliente.Id }, cliente);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.InnerException == null ? "Erro desconhecido. Contate o administrador." : ex.Message });
            }
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            Cliente? cliente = await _clienteService.GetById(id);
            return cliente == null ? NotFound() : Ok(cliente);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] ClienteDto dto)
        {
            try
            {
                Cliente? cliente = await _clienteService.Atualizar(id, dto);
                if (cliente == null) return NotFound();
                return Ok(cliente);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.InnerException == null ? "Erro desconhecido. Contate o administrador." : ex.Message });
            }
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _clienteService.Remover(id);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.InnerException == null ? "Erro desconhecido. Contate o administrador." : ex.Message });
            }
        }
    }
}
