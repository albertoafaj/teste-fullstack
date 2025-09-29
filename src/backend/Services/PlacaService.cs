using Parking.Api.Exceptions;
using System.Text.RegularExpressions;

namespace Parking.Api.Services
{
    public class PlacaService
    {
        public string Sanitizar(string? placa)
        {
            if (string.IsNullOrWhiteSpace(placa))
                throw new BadRequestException($"A placa deve ser informada. Valor nulo/vazio digitado.");

            var p = Regex.Replace(placa, "[^A-Za-z0-9]", "").ToUpperInvariant();
            return p;
        }

        public bool EhValida(string placa)
        {
            if (string.IsNullOrWhiteSpace(placa))
                return false;

            placa = placa.ToUpperInvariant();

            var regexAntiga = new Regex("^[A-Z]{3}[0-9]{4}$");

            var regexMercosul = new Regex("^[A-Z]{3}[0-9][A-Z0-9][0-9]{2}$");

            return regexAntiga.IsMatch(placa) || regexMercosul.IsMatch(placa);
        }
    }
}
