
using Microsoft.EntityFrameworkCore;
using Parking.Api.Data;
using Parking.Api.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Parking.Api.Services
{
    public class FaturamentoService
    {
        private string _observacaoFatura = string.Empty;
        private readonly AppDbContext _db;
        public FaturamentoService(AppDbContext db) => _db = db;

        // BUG proposital: usa dono ATUAL do veículo em vez do dono NA DATA DE CORTE
        public async Task<List<Fatura>> GerarAsync(string competencia, CancellationToken ct = default)
        {
            // competencia formato yyyy-MM

            var part = competencia.Split('-');
            var ano = int.Parse(part[0]);
            var mes = int.Parse(part[1]);
            var ultimoDia = DateTime.DaysInMonth(ano, mes);
            var corte = new DateTime(ano, mes, ultimoDia, 23, 59, 59, DateTimeKind.Utc);
            var primeiroDia = new DateTime(ano, mes, 01, 00, 00, 00, DateTimeKind.Utc);

            var mensalistas = await _db.Clientes
                .Where(c => c.Mensalista)
                .AsNoTracking()
                .ToListAsync(ct);

            var criadas = new List<Fatura>();

            foreach (var cli in mensalistas)
            {
                var existente = await _db.Faturas
                    .FirstOrDefaultAsync(f => f.ClienteId == cli.Id && f.Competencia == competencia, ct);
                if (existente != null) continue; // idempotência simples

                List<Veiculo> veiculosDoCLienteNaCompetencia = await _db.Veiculos
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

                _db.Faturas.Add(fat);
                criadas.Add(fat);
                _observacaoFatura = string.Empty;
            }

            await _db.SaveChangesAsync(ct);
            return criadas;
        }

        public decimal CalculaValorFatura(DateTime dataInicial, DateTime dataCorte, List<Veiculo> veiculosDoCLiente, decimal? valorMensalidade)
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

        private void GerarObservasaoFatura(string placa, decimal valor, int diarias )
        {
            _observacaoFatura += $" Veículo placa: {placa} Valor cobrado: R$ {Math.Round(valor,2)} número de diárias {diarias} diárias;\n"; 
        }
    }
}
