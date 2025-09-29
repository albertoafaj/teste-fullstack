using Parking.Api.Dtos;
using Parking.Api.Exceptions;
using Parking.Api.Models;
using System.Linq.Expressions;

namespace Parking.Api.Services.Interfaces;

public interface IVeiculosService
{
    /// <summary>
    /// Lista todos os veículos, podendo filtrar por cliente.
    /// </summary>
    /// <param name="clienteId">Identificador do cliente (opcional).</param>
    /// <returns>Lista de veículos.</returns>
    Task<List<Veiculo>> Listar(Guid? clienteId = null);

    /// <summary>
    /// Obtém um veículo pelo identificador.
    /// </summary>
    /// <param name="id">ID do veículo.</param>
    /// <returns>O veículo encontrado.</returns>
    /// <exception cref="NotFoundException">Se o veículo não for encontrado.</exception>
    Task<Veiculo> GetById(Guid id);
    
    /// <summary>
    /// Cria um novo veículo.
    /// </summary>
    /// <param name="dto">Dados do veículo.</param>
    /// <returns>O veículo criado.</returns>
    /// <exception cref="BadRequestException">Se a placa for inválida.</exception>
    /// <exception cref="ConflictException">Se a placa já estiver associada a outro cliente.</exception>
    Task<Veiculo> Criar(VeiculoDto dto);
    
    /// <summary>
    /// Atualiza os dados de um veículo existente.
    /// </summary>
    /// <param name="id">ID do veículo.</param>
    /// <param name="dto">Novos dados do veículo.</param>
    /// <returns>O veículo atualizado.</returns>
    /// <exception cref="NotFoundException">Se o veículo não for encontrado.</exception>
    /// <exception cref="BadRequestException">Se a placa for inválida.</exception>
    /// <exception cref="ConflictException">Se a placa for alterada ou já estiver em uso.</exception>
    Task<Veiculo?> Atualizar(Guid id, VeiculoDto dto);
    
    /// <summary>
    /// Remove um veículo pelo identificador.
    /// </summary>
    /// <param name="id">ID Sdo veículo.</param>
    /// <exception cref="NotFoundException">Se o veículo não for encontrado.</exception>   
    Task Remover(Guid id);

    /// <summary>
    /// Verifica se já existe um veículo com base em uma condição.
    /// </summary>
    /// <param name="predicate">Expressão para filtro.</param>
    /// <returns><c>true</c> se existir; caso contrário, <c>false</c>.</returns>
    Task<bool> VerificarSeExiste(Expression<Func<Veiculo, bool>> predicate);
}
