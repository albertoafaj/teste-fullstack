using Parking.Api.Dtos;
using Parking.Api.Models;
using System.Linq.Expressions;

namespace Parking.Api.Services.Interfaces;

public interface IUploadCsvService
{
    Task ImportarCSV(IFormFile file);
    int ListarProcessados();
    int ListarInseridos();
    List<string> ListarErros();
}
