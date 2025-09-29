using Parking.Api.Dtos;
using Parking.Api.Models;
using System.Linq.Expressions;

namespace Parking.Api.Services.Interfaces;

public interface IUploadCsvService
{
    /// <summary>
    /// Importa os dados do CSV informado, criando clientes e veículos conforme necessário.
    /// </summary>
    /// <param name="file">Arquivo CSV a ser importado.</param>
    /// <returns>Tarefa assíncrona.</returns>
    /// <remarks>
    /// O CSV deve estar no formato: 
    /// placa,modelo,ano,cliente_identificador,cliente_nome,cliente_telefone,cliente_endereco,mensalista,valor_mensalidade
    /// </remarks>
    Task ImportarCSV(IFormFile file);

    /// <summary>
    /// Retorna a quantidade de registros processados na última importação.
    /// </summary>
    /// <returns>Número de registros processados.</returns>

    int ListarProcessados();

    /// <summary>
    /// Retorna a quantidade de registros inseridos com sucesso na última importação.
    /// </summary>
    /// <returns>Número de registros inseridos.</returns>
    int ListarInseridos();

    /// <summary>
    /// Lista os erros encontrados durante a última importação.
    /// </summary>
    /// <returns>Lista de mensagens de erro.</returns>
    List<string> ListarErros();
}
