using Microsoft.EntityFrameworkCore;
using Parking.Api.Exceptions;
using Parking.Api.Data;
using Parking.Api.Dtos;
using Parking.Api.Models;
using System.Linq.Expressions;
using Parking.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;

namespace Parking.Api.Services;

public class VeiculosService(AppDbContext db, PlacaService placaService, IClienteService clienteService) : IVeiculosService
{
    public async Task<Veiculo?> Atualizar(Guid id, VeiculoDto dto)
    {
        try
        {
            Veiculo veiculo = await GetById(id);
            if (veiculo.ClienteId != dto.ClienteId)
                await AtualizarCliente(veiculo, dto);
            else
                await AtualizarDadosDoVeiculo(veiculo, dto);
            return veiculo;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<Veiculo> Criar(VeiculoDto dto)
    {
        try
        {
            var placa = placaService.Sanitizar(dto.Placa);
            if (!placaService.EhValida(placa)) throw new BadRequestException($"Placa {placa} inválida.");
            if (await VerificarSeExiste(v => v.Placa == placa && v.DataVigencia == null)) throw new ConflictException($"Placa {placa} já está associada a outro cliente. Para trocar de cliente vá até a tela de veiculos e realize a traoca.");
            var v = new Veiculo { Placa = placa, Modelo = dto.Modelo, Ano = dto.Ano, ClienteId = dto.ClienteId };
            db.Veiculos.Add(v);
            await db.SaveChangesAsync();
            return v;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<Veiculo> GetById(Guid id)
    {
        try
        {
            Veiculo? veiculo = await db.Veiculos.FindAsync(id);
            if (veiculo == null)
                throw new NotFoundException($"Veiculo de id {id} não encontrado");
            return veiculo;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<List<Veiculo>> Listar(Guid? clienteId = null)
    {
        try
        {
            var q = db.Veiculos.AsQueryable();
            if (clienteId.HasValue) q = q.Where(v => v.ClienteId == clienteId.Value);
            return await q.OrderBy(v => v.Placa).ToListAsync();
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
            Veiculo? veiculo = await GetById(id);
            db.Veiculos.Remove(veiculo);
            await db.SaveChangesAsync();
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<bool> VerificarSeExiste(Expression<Func<Veiculo, bool>> predicate)
    {
        try
        {
            return await db.Veiculos.AnyAsync(predicate);
        }
        catch (Exception)
        {
            throw;
        }
    }

    private async Task<Veiculo> AtualizarCliente(Veiculo veiculo, VeiculoDto dto)
    {
        IDbContextTransaction transaction = await db.Database.BeginTransactionAsync();
        try
        {
            await clienteService.GetById(dto.ClienteId);
            veiculo.DataVigencia = DateTime.UtcNow;
            await db.SaveChangesAsync();
            Veiculo veiculoAtualizado = await Criar(dto);
            await transaction.CommitAsync();
            await transaction.DisposeAsync();
            return veiculoAtualizado;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }

    }
    private async Task<Veiculo> AtualizarDadosDoVeiculo(Veiculo veiculo, VeiculoDto dto)
    {
        var placa = placaService.Sanitizar(dto.Placa);
        if (!placaService.EhValida(placa)) throw new BadRequestException($"Placa {placa} inválida.");

        if (veiculo.Placa != dto.Placa) throw new ConflictException($"A placa do veículo não pode ser alterada.");

        veiculo.Modelo = dto.Modelo;
        veiculo.Ano = dto.Ano;
        await db.SaveChangesAsync();
        return veiculo;
    }
}
