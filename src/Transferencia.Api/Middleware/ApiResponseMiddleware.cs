using System.Text.Json; 
using Transferencia.Application.Interfaces;

namespace Transferencia.Api.Middleware
{
    public class ApiResponseMiddleware
    {
        private readonly RequestDelegate _next;

        public ApiResponseMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            using (var memoryStream = new MemoryStream())
            {
                var originalBodyStream = context.Response.Body;
                context.Response.Body = memoryStream;

                try
                {
                    await _next(context);

                    memoryStream.Seek(0, SeekOrigin.Begin);

                    var responseBody = await new StreamReader(memoryStream).ReadToEndAsync();

                    var apiResponse = new ApiResponse<object>
                    {
                        Success = context.Response.StatusCode >= 200 && context.Response.StatusCode < 300,
                        Data = responseBody.Length > 0 ? JsonSerializer.Deserialize<object>(responseBody) : null,
                        Error = null
                    };

                    context.Response.Body = originalBodyStream;
                    context.Response.ContentType = "application/json";

                    var jsonResponse = JsonSerializer.Serialize(apiResponse); 
                    await context.Response.WriteAsync(jsonResponse);
                }
                catch (Exception ex)
                {
                    context.Response.Body = originalBodyStream;
                    throw;
                }
            }
        }
    }
}
