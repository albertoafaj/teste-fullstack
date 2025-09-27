using Parking.Api.Exceptions;
using System.Net;
using System.Text.Json;

namespace Parking.Api.Middlewares
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            HttpStatusCode status;
            string message = exception.Message;

            switch (exception)
            {
                case BadRequestException:
                    status = HttpStatusCode.BadRequest;
                    break;
                case NotFoundException:
                    status = HttpStatusCode.NotFound;
                    break;
                case ConflictException:
                    status = HttpStatusCode.Conflict;
                    break;
                default:
                    status = HttpStatusCode.InternalServerError;
                    message = "Erro desconhecido. Contate o administrador.";
                    break;
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)status;

            var result = JsonSerializer.Serialize(new { message });
            return context.Response.WriteAsync(result);
        }
    }
}
