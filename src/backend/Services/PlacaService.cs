using Parking.Api.Exceptions;
using System.Text.RegularExpressions;

namespace Parking.Api.Services;

/// <summary>
/// Servi�o respons�vel por sanitizar e validar placas de ve�culos
/// nos formatos tradicional e Mercosul.
/// </summary>
public class PlacaService
{
    /// <summary>
    /// Remove caracteres n�o alfanum�ricos da placa e normaliza para mai�sculo.
    /// </summary>
    /// <param name="placa">Placa a ser sanitizada.</param>
    /// <returns>Placa limpa em mai�sculo.</returns>
    /// <exception cref="BadRequestException">Se a placa for nula ou vazia.</exception>
    public string Sanitizar(string? placa)
    {
        if (string.IsNullOrWhiteSpace(placa))
            throw new BadRequestException($"A placa deve ser informada. Valor nulo/vazio digitado.");

        var p = Regex.Replace(placa, "[^A-Za-z0-9]", "").ToUpperInvariant();
        return p;
    }

    /// <summary>
    /// Verifica se a placa � v�lida no padr�o antigo (LLLNNNN) 
    /// ou Mercosul (LLLNLNN).
    /// </summary>
    /// <param name="placa">Placa a ser validada.</param>
    /// <returns>true se v�lida, false caso contr�rio.</returns>
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
