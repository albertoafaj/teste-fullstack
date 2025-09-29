using Microsoft.AspNetCore.Mvc;
using Parking.Api.Dtos;
using Parking.Api.Models;
using Parking.Api.Services.Interfaces;

namespace Parking.Api.Controllers;

/// <summary>
/// Controlador responsável por gerenciar os clientes do estacionamento.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ClientesController : ControllerBase
{
    IClienteService _clienteService;

    /// <summary>
    /// Inicializa uma nova instância de <see cref="ClientesController"/>.
    /// </summary>
    /// <param name="clienteService">Serviço responsável pelas operações de cliente.</param>
    public ClientesController(IClienteService clienteService)
    {
        _clienteService = clienteService;
    }

    /// <summary>
    /// Lista clientes com suporte a paginação e filtro.
    /// </summary>
    /// <param name="pagina">Número da página (padrão: 1).</param>
    /// <param name="tamanho">Quantidade de itens por página (padrão: 10).</param>
    /// <param name="filtro">Filtro opcional para busca pelo nome ou outros campos.</param>
    /// <param name="mensalista">Filtro para clientes mensalistas ("all", "true", "false").</param>
    /// <returns>Uma lista paginada de clientes.</returns>
    /// <response code="200">Retorna a lista de clientes.</response>
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] int pagina = 1, [FromQuery] int tamanho = 10, [FromQuery] string? filtro = null, [FromQuery] string mensalista = "all")
    {
        (int total, List<Cliente> itens) result = await _clienteService.Listar(pagina, tamanho, filtro, mensalista);
        return Ok(new { result.total, result.itens });
    }

    /// <summary>
    /// Cria um novo cliente.
    /// </summary>
    /// <param name="dto">Dados do cliente a serem criados.</param>
    /// <returns>O cliente criado.</returns>
    /// <response code="201">Retorna o cliente criado.</response>
    /// <response code="400">Se os dados enviados forem inválidos.</response>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ClienteDto dto)
    {
        Cliente cliente = await _clienteService.Criar(dto);
        return CreatedAtAction(nameof(GetById), new { id = cliente.Id }, cliente);
    }

    /// <summary>
    /// Obtém um cliente pelo ID.
    /// </summary>
    /// <param name="id">ID  do cliente.</param>
    /// <returns>O cliente encontrado.</returns>
    /// <response code="200">Retorna o cliente encontrado.</response>
    /// <response code="404">Se o cliente não for encontrado.</response>
    /// <response code="409">Não será possível cadastrar/atualizar o cliente caso exista um cadastro para {dto.Nome} no telefone:({dto?.Telefone}.</response>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        Cliente cliente = await _clienteService.GetById(id);
        return Ok(cliente);
    }

    /// <summary>
    /// Atualiza os dados de um cliente existente.
    /// </summary>
    /// <param name="id">ID do cliente.</param>
    /// <param name="dto">Novos dados do cliente.</param>
    /// <returns>O cliente atualizado.</returns>
    /// <response code="200">Retorna o cliente atualizado.</response>
    /// <response code="400">Se os dados enviados forem inválidos.</response>
    /// <response code="404">Se o cliente não for encontrado.</response>
    /// <response code="409">Não será possível cadastrar/atualizar o cliente caso exista um cadastro para {dto.Nome} no telefone:({dto?.Telefone}.</response>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] ClienteDto dto)
    {
        Cliente? cliente = await _clienteService.Atualizar(id, dto);
        return Ok(cliente);
    }

    /// <summary>
    /// Remove um cliente.
    /// </summary>
    /// <param name="id">Identificador ID</param>
    /// <returns>Resposta sem conteúdo em caso de sucesso.</returns>
    /// <response code="200">Retorna o cliente atualizado.</response>
    /// <response code="400">Se os dados enviados forem inválidos.</response>
    /// <response code="404">Se o cliente não for encontrado.</response>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _clienteService.Remover(id);
        return NoContent();
    }
}
