
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Parking.Api.Data;
using Parking.Api.Dtos;
using Parking.Api.Exceptions;
using Parking.Api.Models;
using Parking.Api.Services.Interfaces;
using System.Globalization;
using System.Linq.Expressions;

namespace Parking.Api.Services
{
    public class FaturamentoService(AppDbContext db) : IFaturamentoService
    {
        private string _observacaoFatura = string.Empty;

        public async Task<List<Fatura>> GerarAsync(string competencia, CancellationToken ct = default)
        {
            if (!ECompetenciaValida(competencia)) throw new BadRequestException($"A competência deve ser digitada no formato yyyy-MM (ex.: 2025-09)");

            var part = competencia.Split('-');
            var ano = int.Parse(part[0]);
            var mes = int.Parse(part[1]);
            var ultimoDia = DateTime.DaysInMonth(ano, mes);
            var corte = new DateTime(ano, mes, ultimoDia, 23, 59, 59, DateTimeKind.Utc);
            var primeiroDia = new DateTime(ano, mes, 01, 00, 00, 00, DateTimeKind.Utc);

            var mensalistas = await db.Clientes
                .Where(c => c.Mensalista)
                .AsNoTracking()
                .ToListAsync(ct);

            var criadas = new List<Fatura>();

            foreach (var cli in mensalistas)
            {
                var existente = await db.Faturas
                    .FirstOrDefaultAsync(f => f.ClienteId == cli.Id && f.Competencia == competencia, ct);
                if (existente != null) continue; // idempotência simples

                List<Veiculo> veiculosDoCLienteNaCompetencia = await db.Veiculos
                    .Where(v => v.ClienteId == cli.Id && (v.DataInclusao < corte && v.DataVigencia == null) || (v.DataVigencia != null && v.DataVigencia > primeiroDia))
                    .ToListAsync(ct);

                var fat = new Fatura
                {
                    Competencia = competencia,
                    ClienteId = cli.Id,
                    Valor = CalculaValorFatura(dataInicial: primeiroDia, dataCorte: corte, veiculosDoCLiente: veiculosDoCLienteNaCompetencia, valorMensalidade: cli.ValorMensalidade),
                    Observacao = _observacaoFatura
                };

                foreach (var id in veiculosDoCLienteNaCompetencia.Select(v => v.Id).ToList())
                    fat.Veiculos.Add(new FaturaVeiculo { FaturaId = fat.Id, VeiculoId = id });

                db.Faturas.Add(fat);
                criadas.Add(fat);
                _observacaoFatura = string.Empty;
            }

            await db.SaveChangesAsync(ct);
            return criadas;
        }

        /// <summary>
        /// Calcula o valor da fatura de um cliente com base nos veículos e dias ativos no mês.
        /// </summary>
        /// <param name="dataInicial">Data inicial do período.</param>
        /// <param name="dataCorte">Data final do período.</param>
        /// <param name="veiculosDoCLiente">Veículos do cliente.</param>
        /// <param name="valorMensalidade">Valor da mensalidade do cliente.</param>
        /// <returns>Valor total da fatura.</returns>
        private decimal CalculaValorFatura(DateTime dataInicial, DateTime dataCorte, List<Veiculo> veiculosDoCLiente, decimal? valorMensalidade)
        {
            decimal valorFatura = 0m;
            int numeroDiasMes = DateTime.DaysInMonth(dataCorte.Year, dataCorte.Month);
            decimal diaria = 0m;
            if (valorMensalidade.HasValue && valorMensalidade > 0m)
                diaria = valorMensalidade.Value / numeroDiasMes;

            foreach (Veiculo veiculo in veiculosDoCLiente)
            {
                DateTime inicio = veiculo.DataInclusao > dataInicial ? veiculo.DataInclusao : dataInicial;
                DateTime fim = veiculo.DataVigencia.HasValue && veiculo.DataVigencia.Value < dataCorte
                    ? veiculo.DataVigencia.Value
                    : dataCorte;

                int diasACobrar = (fim - inicio).Days + 1;

                valorFatura += diaria * diasACobrar;
                GerarObservasaoFatura(veiculo.Placa, diaria * diasACobrar, diasACobrar);
            }
            return valorFatura;
        }

        private void GerarObservasaoFatura(string placa, decimal valor, int diarias)
        {
            _observacaoFatura += $" Veículo placa: {placa} Valor cobrado: R$ {Math.Round(valor, 2)} número de diárias {diarias} diárias;\n";
        }

        public async Task<List<FaturaDto>> Listar([FromQuery] string? competencia = null)
        {
            try
            {
                IQueryable<Fatura> q = db.Faturas.AsQueryable();
                if (!string.IsNullOrWhiteSpace(competencia)) q = q.Where(f => f.Competencia == competencia);
                List<Fatura> faturas = await q.OrderByDescending(v => v.CriadaEm).ToListAsync();
                List<FaturaDto> faturasDto = [];

                foreach (var f in faturas)
                {
                    int qtdVeiculos = await ObterFaturasVeiculos(x => x.FaturaId == f.Id);

                    faturasDto.Add(new FaturaDto(f.Id,f.Competencia,f.ClienteId,f.Valor,f.CriadaEm,qtdVeiculos));
                }
                return faturasDto;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<int> ObterFaturasVeiculos(Expression<Func<FaturaVeiculo, bool>> predicate)
        {
            try
            {
                return await db.FaturasVeiculos.Where(predicate).CountAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<string>> ListarPlacasPorFaturaId(Guid id)
        {
            try
            {
                return await db.FaturasVeiculos
                    .Where(x => x.FaturaId == id)
                    .Join(db.Veiculos, fv => fv.VeiculoId, v => v.Id, (fv, v) => v.Placa)
                    .ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Verifica se uma competência está no formato válido yyyy-MM.
        /// </summary>
        /// <param name="competencia">Competência a ser validada.</param>
        /// <returns>true se válida, false caso contrário.</returns>
        public bool ECompetenciaValida(string competencia)
        {
            return DateTime.TryParseExact(
                competencia,
                "yyyy-MM",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out _
            );
        }
    }
}