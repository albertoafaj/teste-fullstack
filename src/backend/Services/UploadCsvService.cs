using Microsoft.EntityFrameworkCore.Storage;
using Parking.Api.Data;
using Parking.Api.Dtos;
using Parking.Api.Models;
using Parking.Api.Services.Interfaces;
using System.Text;

namespace Parking.Api.Services;

public class UploadCsvService(AppDbContext db, IVeiculosService veiculosService, IClienteService clienteService) : IUploadCsvService
{
    private int Processados { get; set; }
    private int Inseridos { get; set; }
    private List<string> Erros { get; set; } = [];
    public async Task ImportarCSV(IFormFile file)
    {
        try
        {
            using var stream = file.OpenReadStream();
            using var reader = new StreamReader(stream, Encoding.UTF8);

            int linha = 0;
            string? header = await reader.ReadLineAsync(); // consome cabeçalho
            
            while (!reader.EndOfStream)
            {
                linha++;
                var raw = await reader.ReadLineAsync();
                if (string.IsNullOrWhiteSpace(raw)) continue;
                Processados++;
                IDbContextTransaction transaction = await db.Database.BeginTransactionAsync();

                // CSV simples separado por vírgula: placa,modelo,ano,cliente_identificador,cliente_nome,cliente_telefone,cliente_endereco,mensalista,valor_mensalidade
                var cols = raw.Split(',');
                try
                {
                    Guid cliId = Guid.Empty;
                    Guid.TryParse(cols[3], out cliId);
                    ClienteDto clienteDto = new(
                        Nome: cols[4],
                        Telefone: new string((cols[5] ?? "").Where(char.IsDigit).ToArray()),
                        Endereco: cols[6],
                        Mensalista: bool.TryParse(cols[7], out var m) && m,
                        ValorMensalidade: decimal.TryParse(cols[8], out var vm) ? vm : null
                        );

                    Cliente? cliente = await clienteService.ObterClientePorNomeTelefone(clienteDto);

                    if (cliente == null)
                        cliente = await clienteService.Criar(clienteDto);

                    VeiculoDto veiculoDto = new(
                        Placa: cols[0],
                        Modelo: cols[1],
                        Ano: int.TryParse(cols[2], out var _ano) ? _ano : null,
                        ClienteId: cliente.Id
                        );

                    await veiculosService.Criar(veiculoDto);

                    await transaction.CommitAsync();
                    await transaction.DisposeAsync();

                    Inseridos++;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    Erros.Add($"Linha {linha}: {ex.Message} (raw='{raw}')");
                }
            }
        }
        catch (Exception)
        {

            throw;
        }
    }

    public List<string> ListarErros()
    {
        return Erros;
    }

    public int ListarInseridos()
    {
        return Inseridos;
    }

    public int ListarProcessados()
    {
        return Processados;
    }
}
