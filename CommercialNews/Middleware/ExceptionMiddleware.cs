using System.Net;
using System.Text.Json;
using Application.Common.Exceptions;


namespace CommercialNews.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IHostEnvironment _env;

        // Tái sử dụng JsonOption camelCase
        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (AppException appEx)
            {
                _logger.LogWarning(appEx, $"AppException: {appEx.ErrorCode}");

                context.Response.ContentType = "application/json";
                context.Response.StatusCode = appEx.StatusCode;

                var traceId = context.TraceIdentifier ?? Guid.NewGuid().ToString();

                var response = new
                {
                    statusCode = appEx.StatusCode,
                    errorCode = appEx.ErrorCode,
                    message = appEx.Message,
                    traceId
                };

                await context.Response.WriteAsync(JsonSerializer.Serialize(response, _jsonOptions));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);

                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                var traceId = context.TraceIdentifier ?? Guid.NewGuid().ToString();

                object response;

                if (_env.IsDevelopment())
                {
                    response = new
                    {
                        statusCode = context.Response.StatusCode,
                        errorCode = "INTERNAL_SERVER_ERROR",
                        message = ex.Message,
                        details = ex.StackTrace,
                        traceId
                    };
                }
                else
                {
                    response = new
                    {
                        statusCode = context.Response.StatusCode,
                        errorCode = "INTERNAL_SERVER_ERROR",
                        message = "Internal Server Error",
                        traceId
                    };
                }

                await context.Response.WriteAsync(JsonSerializer.Serialize(response, _jsonOptions));
            }

        }
    }
}
