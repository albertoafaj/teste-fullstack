using Microsoft.EntityFrameworkCore;
using Parking.Api.Data;
using Parking.Api.Dtos;
using Parking.Api.Exceptions;
using Parking.Api.Models;
using System.Linq.Expressions;

namespace Parking.Api.Services;

public class ClienteService(AppDbContext db) : IClienteService
{
    public async Task<Cliente?> Atualizar(Guid id, ClienteDto dto)
    {
        try
        {
            await VerificarUnicidadeClienteNomeTelefone(dto);
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
            await VerificarUnicidadeClienteNomeTelefone(dto);

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

    public async Task<bool> VerificarSeExiste(Expression<Func<Cliente, bool>> predicate)
    {
        try
        {
            return await db.Clientes.AnyAsync(predicate);
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task VerificarUnicidadeClienteNomeTelefone(ClienteDto dto)
    {
        bool existe = await VerificarSeExiste(c => c.Nome == dto.Nome && c.Telefone == dto.Telefone);
        if (existe) throw new ConflictException($"Não será possível cadastrar/atualizar o cliente. Já existe um cadastro para {dto.Nome} no telefone:({dto.Telefone})");
    }
}
