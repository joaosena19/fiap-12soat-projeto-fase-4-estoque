using API.Dtos;
using API.Extensions;
using Shared.Exceptions;
using System.Net;
using System.Text.Json;

namespace API.Middleware
{
    /// <summary>
    /// Middleware para tratamento centralizado de exceções
    /// </summary>
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
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

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            ErrorResponseDto response;

            switch (exception)
            {
                case DomainException domainEx:
                    _logger.LogWarning(domainEx, "Domain exception occurred: {Message}", domainEx.Message);
                    var httpStatusCode = domainEx.ErrorType.ToHttpStatusCode();
                    response = new ErrorResponseDto(domainEx.Message, (int)httpStatusCode);
                    context.Response.StatusCode = (int)httpStatusCode;
                    break;

                default:
                    _logger.LogError(exception, "An unexpected error occurred: {Message}", exception.Message);
                    response = new ErrorResponseDto("Ocorreu um erro interno no servidor.", (int)HttpStatusCode.InternalServerError);
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    break;
            }

            var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await context.Response.WriteAsync(jsonResponse);
        }
    }
}
