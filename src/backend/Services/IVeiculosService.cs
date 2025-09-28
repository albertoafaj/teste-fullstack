using Parking.Api.Dtos;
using Parking.Api.Models;
using System.Linq.Expressions;

namespace Parking.Api.Services;

public interface IVeiculosService
{
    Task<List<Veiculo>> Listar(Guid? clienteId = null);
    Task<Veiculo> GetById(Guid id);
    Task<Veiculo> Criar(VeiculoDto dto);
    Task<Veiculo?> Atualizar(Guid id, VeiculoDto dto);
    Task Remover(Guid id);
    Task<bool> VerificarSeExiste(Expression<Func<Veiculo, bool>> predicate);
}
