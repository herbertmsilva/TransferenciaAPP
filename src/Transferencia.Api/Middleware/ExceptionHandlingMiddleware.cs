using System.Text.Encodings.Web; 
using System.Text.Json; 
using System.Text.Unicode;
using Transferencia.Application.Exceptions;
using Transferencia.Application.Interfaces;

namespace Transferencia.Api.Middleware
{
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
            catch (CustomException ex)
            {
                _logger.LogWarning("CustomException capturada: {Message}", ex.Message);
                await HandleCustomExceptionAsync(context, ex);
            }
            catch (FluentValidation.ValidationException ex)
            {
                _logger.LogWarning("ValidationException capturada: {Message}", ex.Message);
                await HandleValidationException(context, ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exceção não tratada capturada");
                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleCustomExceptionAsync(HttpContext context, CustomException ex)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = ex.StatusCode;

            var response = ApiResponse<object>.ErrorResponse(
                ex.StatusCode.ToString(),
                ex.Message);

            var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
            });

            return context.Response.WriteAsync(jsonResponse);
        }

        private Task HandleValidationException(HttpContext context, FluentValidation.ValidationException ex)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status400BadRequest;

            var errors = ex.Errors.Select(error => new
            {
                Field = error.PropertyName,
                Error = error.ErrorMessage
            });

            var response = ApiResponse<object>.ErrorResponse(
                StatusCodes.Status400BadRequest.ToString(),
                "Erros de validação ocorreram.",
                errors: errors);

            var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
            });

            return context.Response.WriteAsync(jsonResponse);
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;

            
            var isDevelopment = context.RequestServices.GetService<IWebHostEnvironment>().IsDevelopment();

            var response = isDevelopment
                ? ApiResponse<object>.ErrorResponse(
                    StatusCodes.Status500InternalServerError.ToString(),
                    "Ocorreu um erro interno no servidor.",
                    details: exception.ToString()) 
                : ApiResponse<object>.ErrorResponse(
                    StatusCodes.Status500InternalServerError.ToString(),
                    "Ocorreu um erro interno no servidor.");

            var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
            });

            return context.Response.WriteAsync(jsonResponse);
        }
    }
}
