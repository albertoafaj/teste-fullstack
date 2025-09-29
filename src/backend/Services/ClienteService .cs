using Microsoft.EntityFrameworkCore;
using Parking.Api.Data;
using Parking.Api.Dtos;
using Parking.Api.Exceptions;
using Parking.Api.Models;
using Parking.Api.Services.Interfaces;
using System.Linq.Expressions;

namespace Parking.Api.Services;

public class ClienteService(AppDbContext db) : IClienteService
{
    public async Task<Cliente?> Atualizar(Guid id, ClienteDto dto)
    {
        try
        {
            await ValidarCliente(dto);
            Cliente cliente = await GetById(id);
            cliente.Nome = dto.Nome;
            cliente.Telefone = dto.Telefone;
            cliente.Endereco = dto.Endereco;
            cliente.Mensalista = dto.Mensalista;
            cliente.ValorMensalidade = dto.ValorMensalidade;
            await db.SaveChangesAsync();
            return cliente;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<Cliente> Criar(ClienteDto dto)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(dto.Nome))
                throw new NotFoundException("Não foi possível criar o usuário. O nome informado é vázio ou nulo");
            await ValidarCliente(dto);

            var c = new Cliente
            {
                Nome = dto.Nome,
                Telefone = dto.Telefone,
                Endereco = dto.Endereco,
                Mensalista = dto.Mensalista,
                ValorMensalidade = dto.ValorMensalidade,
            };
            db.Clientes.Add(c);
            await db.SaveChangesAsync();
            return c;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<Cliente> GetById(Guid id)
    {
        try
        {
            Cliente? c = await db.Clientes.Include(x => x.Veiculos).FirstOrDefaultAsync(x => x.Id == id);
            if (c == null)
                throw new NotFoundException($"Cliente de id {id} não encontrado");
            return c;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<(int total, List<Cliente> itens)> Listar(int pagina, int tamanho, string? filtro, string mensalista)
    {
        try
        {
            var q = db.Clientes.AsQueryable();
            if (!string.IsNullOrWhiteSpace(filtro))
                q = q.Where(c => c.Nome.Contains(filtro));
            if (mensalista == "true") q = q.Where(c => c.Mensalista);
            if (mensalista == "false") q = q.Where(c => !c.Mensalista);
            var total = await q.CountAsync();
            var itens = await q
                .OrderBy(c => c.Nome)
                .Skip((pagina - 1) * tamanho)
                .Take(tamanho)
                .ToListAsync();
            return (total, itens);
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task Remover(Guid id)
    {
        try
        {
            Cliente? cliente = await db.Clientes.FindAsync(id);
            if (cliente == null) throw new NotFoundException("Não foi possível excluír o cliente. Cliente não encontrado");
            var temVeiculos = await db.Veiculos.AnyAsync(v => v.ClienteId == id);
            if (temVeiculos) throw new BadRequestException("Cliente possui veículos associados. Transfira ou remova antes.");
            db.Clientes.Remove(cliente);
            await db.SaveChangesAsync();
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<IEnumerable<Cliente>> Filtrar(Expression<Func<Cliente, bool>> predicate)
    {
        try
        {
            return await db.Clientes.Where(predicate).Include(x => x.Veiculos).ToListAsync();
        }
        catch (Exception)
        {
            throw;
        }
    }

    /// <summary>
    /// Valida se já existe cliente com o mesmo nome e telefone.
    /// </summary>
    /// <param name="dto">Dados do cliente para validação.</param>
    /// <exception cref="ConflictException">Se já existir cliente duplicado.</exception>
    public async Task ValidarCliente(ClienteDto dto)
    {
        Cliente? cliente = await ObterClientePorNomeTelefone(dto);
        if (cliente != null) throw new ConflictException($"Não será possível cadastrar/atualizar o cliente. Já existe um cadastro para {dto?.Nome} no telefone:({dto?.Telefone})");
    }

    public async Task<Cliente?> ObterClientePorNomeTelefone(ClienteDto dto)
    {
        string? telefoneDto = string.IsNullOrWhiteSpace(dto.Telefone) ? "" : dto.Telefone.Trim();

        IEnumerable<Cliente> clientes = await Filtrar(c => c.Nome == dto.Nome.Trim() && c.Telefone == telefoneDto);

        return clientes.FirstOrDefault();
    }
}
