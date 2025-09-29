
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Parking.Api.Services.Interfaces;

namespace Parking.Api.Controllers;

/// <summary>
/// Controlador respons�vel pela importa��o de arquivos CSV.
/// </summary>
[ApiController]
[Route("api/import")]
public class ImportController(IUploadCsvService uploadCsvService) : ControllerBase
{
    /// <summary>
    /// Importa um arquivo CSV contendo dados para processamento.
    /// </summary>
    /// <remarks>
    /// O arquivo deve ser enviado no campo <c>file</c> de um formul�rio multipart.
    /// </remarks>
    /// <returns>Resumo da importa��o contendo quantidade de processados, inseridos e lista de erros.</returns>
    /// <response code="200">Retorna o resumo da importa��o.</response>
    /// <response code="400">Se nenhum arquivo for enviado ou o formato for inv�lido.</response>
    [HttpPost("csv")]
    public async Task<IActionResult> ImportCsv()
    {
        if (!Request.HasFormContentType || Request.Form.Files.Count == 0)
            return BadRequest("Envie um arquivo CSV no campo 'file'.");
        var file = Request.Form.Files[0];
        await uploadCsvService.ImportarCSV(file);
        int pocessados = uploadCsvService.ListarProcessados();
        int inseridos = uploadCsvService.ListarInseridos();
        List<string> erros = uploadCsvService.ListarErros();
        return Ok(new { pocessados, inseridos, erros });

    }
}
