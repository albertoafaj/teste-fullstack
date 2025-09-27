using Parking.Api.Dtos;
using Parking.Api.Models;
using System.Linq.Expressions;

namespace Parking.Api.Services;

public interface IClienteService
{
    Task<(int total, List<Cliente> itens)> Listar(int pagina, int tamanho, string? filtro, string mensalista);
    Task<Cliente?> GetById(Guid id);
    Task<Cliente> Criar(ClienteDto dto);
    Task<Cliente?> Atualizar(Guid id, ClienteDto dto);
    Task Remover(Guid id);
    Task<bool> VerificarSeExiste(Expression<Func<Cliente, bool>> predicate);
}
