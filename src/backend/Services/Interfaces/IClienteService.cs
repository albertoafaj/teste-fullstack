using Parking.Api.Dtos;
using Parking.Api.Exceptions;
using Parking.Api.Models;
using System.Linq.Expressions;

namespace Parking.Api.Services.Interfaces;

/// <summary>
/// Serviço responsável por operações relacionadas a clientes,
/// incluindo criação, atualização, listagem, remoção e validações.
/// </summary>
public interface IClienteService
{
    /// <summary>
    /// Lista os clientes com suporte a paginação e filtros.
    /// </summary>
    /// <param name="pagina">Número da página.</param>
    /// <param name="tamanho">Quantidade de itens por página.</param>
    /// <param name="filtro">Filtro pelo nome do cliente.</param>
    /// <param name="mensalista">Filtro de mensalista ("true", "false" ou "all").</param>
    /// <returns>Total de clientes e a lista paginada.</returns>
    Task<(int total, List<Cliente> itens)> Listar(int pagina, int tamanho, string? filtro, string mensalista);

    /// <summary>
    /// Obtém um cliente pelo seu identificador.
    /// </summary>
    /// <param name="id">Identificador do cliente.</param>
    /// <returns>Cliente encontrado, incluindo veículos associados.</returns>
    /// <exception cref="NotFoundException">Se o cliente não for encontrado.</exception>

    Task<Cliente> GetById(Guid id);

    /// <summary>
    /// Cria um novo cliente.
    /// </summary>
    /// <param name="dto">Dados do cliente.</param>
    /// <returns>Cliente criado.</returns>
    /// <exception cref="NotFoundException">Se o nome for nulo ou vazio.</exception>
    /// <exception cref="ConflictException">Se já existir um cliente com o mesmo nome e telefone.</exception>
    Task<Cliente> Criar(ClienteDto dto);

    /// <summary>
    /// Atualiza os dados de um cliente existente.
    /// </summary>
    /// <param name="id">Identificador do cliente.</param>
    /// <param name="dto">Dados do cliente para atualização.</param>
    /// <returns>Cliente atualizado.</returns>
    /// <exception cref="NotFoundException">Se o cliente não for encontrado.</exception>
    /// <exception cref="ConflictException">Se já existir um cliente com o mesmo nome e telefone.</exception>
    Task<Cliente?> Atualizar(Guid id, ClienteDto dto);

    /// <summary>
    /// Remove um cliente pelo seu identificador.
    /// </summary>
    /// <param name="id">Identificador do cliente.</param>
    /// <exception cref="NotFoundException">Se o cliente não for encontrado.</exception>
    /// <exception cref="BadRequestException">Se o cliente possuir veículos associados.</exception>
    Task Remover(Guid id);

    /// <summary>
    /// Filtra clientes de acordo com a expressão informada.
    /// </summary>
    /// <param name="predicate">Expressão para filtro.</param>
    /// <returns>Lista de clientes filtrados, incluindo veículos.</returns>
    Task<IEnumerable<Cliente>> Filtrar(Expression<Func<Cliente, bool>> predicate);

    /// <summary>
    /// Obtém cliente por nome e telefone.
    /// </summary>
    /// <param name="dto">Dados contendo nome e telefone.</param>
    /// <returns>Cliente encontrado ou null.</returns>
    Task<Cliente?> ObterClientePorNomeTelefone(ClienteDto dto);
}
