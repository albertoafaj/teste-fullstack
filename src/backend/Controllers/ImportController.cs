
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Parking.Api.Data;
using Parking.Api.Models;
using Parking.Api.Services;
using Parking.Api.Services.Interfaces;
using System.Text;

namespace Parking.Api.Controllers
{
    [ApiController]
    [Route("api/import")]
    public class ImportController(IUploadCsvService uploadCsvService) : ControllerBase
    {
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
}
