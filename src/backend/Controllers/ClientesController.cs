
using Microsoft.AspNetCore.Mvc;
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
            (int total, List<Cliente> itens) result = await _clienteService.Listar(pagina, tamanho, filtro, mensalista);
            return Ok(new { result.total, result.itens });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ClienteDto dto)
        {
            Cliente cliente = await _clienteService.Criar(dto);
            return CreatedAtAction(nameof(GetById), new { id = cliente.Id }, cliente);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            Cliente cliente = await _clienteService.GetById(id);
            return Ok(cliente);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] ClienteDto dto)
        {
            Cliente? cliente = await _clienteService.Atualizar(id, dto);
            return Ok(cliente);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _clienteService.Remover(id);
            return NoContent();
        }
    }
}
