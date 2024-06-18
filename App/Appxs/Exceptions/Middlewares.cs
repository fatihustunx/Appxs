using System;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using App.Appxs.Apps.Logging.Usings;

namespace App.Appxs.Exceptions
{
    public class Middlewares
    {
        private readonly ILogger _logger;

        private readonly RequestDelegate _next;

        private readonly IHttpContextAccessor _httpContextAccessor;

        public Middlewares(ILogger logger, RequestDelegate next,
            IHttpContextAccessor httpContextAccessor)
        {
            _next = next;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception exception)
            {
                await LogException(context, exception);

                await HandleExceptionAsync(context, exception);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            if (exception.GetType() == typeof(AuthorizationException))
                return CreateAuthorizationException(context, exception);
            if (exception.GetType() == typeof(ValidationException)) return CreateValidationException(context, exception);
            if (exception.GetType() == typeof(BusinessException)) return CreateBusinessException(context, exception);
            return CreateInternalException(context, exception);
        }

        private Task CreateAuthorizationException(HttpContext context, Exception exception)
        {
            context.Response.StatusCode = Convert.ToInt32(HttpStatusCode.Unauthorized);

            return context.Response.WriteAsync(new AuthorizationProblemDetails
            {
                Status = StatusCodes.Status401Unauthorized,
                Type = "https://example.com/probs/authorization",
                Title = "Authorization exception",
                Detail = exception.Message,
                Instance = ""
            }.ToString());
        }

        private Task CreateValidationException(HttpContext context, Exception exception)
        {
            context.Response.StatusCode = Convert.ToInt32(HttpStatusCode.BadRequest);
            object errors = ((ValidationException)exception).Errors;

            return context.Response.WriteAsync(new ValidationProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Type = "https://example.com/probs/validation",
                Title = "Validation Error(s)",
                Detail = "",
                Instance = "",
                Errors = errors
            }.ToString());
        }

        private Task CreateBusinessException(HttpContext context, Exception exception)
        {
            context.Response.StatusCode = Convert.ToInt32(HttpStatusCode.BadRequest);

            return context.Response.WriteAsync(new BusinessProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Type = "https://example.com/probs/business",
                Title = "Business Exceptions",
                Detail = exception.Message,
                Instance = ""
            }.ToString());
        }

        private Task CreateInternalException(HttpContext context, Exception exception)
        {
            context.Response.StatusCode = Convert.ToInt32(HttpStatusCode.InternalServerError);

            ProblemDetails res = new()
            {
                Status = StatusCodes.Status500InternalServerError,
                Type = "https://example.com/probs/internal",
                Title = "Internal Server Errors",
                Detail = exception.Message,
                Instance = ""
            };

            var s = JsonSerializer.Serialize(res);

            return context.Response.WriteAsync(s);
        }

        private Task LogException(HttpContext context, Exception ex)
        {
            List<LogParameter> logParameters = new()
        {
            new LogParameter()
            {
                Type = context.GetType().Name, Value = ex.ToString()
            }
        };

            LogDetailWithException logDetail = new()
            {
                ExceptionMessage = ex.Message,
                MethodName = _next.Method.Name,
                Parameters = logParameters,
                User = _httpContextAccessor.HttpContext.User.Identity?.Name ?? "?"
            };

            _logger.Error(JsonSerializer.Serialize(logDetail));

            return Task.CompletedTask;
        }
    }
}