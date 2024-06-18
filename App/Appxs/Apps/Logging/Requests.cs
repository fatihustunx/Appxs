using App.Appxs.Apps.Logging.Usings;
using MediatR;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

using System;

namespace App.Appxs.Apps.Logging;

public interface ILoggingRequest
{
}

public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>, ILoggingRequest
{
    private readonly ILogger _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public LoggingBehavior(ILogger loggerServiceBase, IHttpContextAccessor httpContextAccessor)
    {
        _logger = loggerServiceBase;
        _httpContextAccessor = httpContextAccessor;
    }

    public Task<TResponse> Handle(TRequest request,
        RequestHandlerDelegate<TResponse> next,
       CancellationToken cancellationToken)
    {
        List<LogParameter> logParameters = new();
        logParameters.Add(new LogParameter
        {
            Type = request.GetType().Name,
            Value = request
        });

        LogDetail logDetail = new()
        {
            MethodName = next.Method.Name,
            Parameters = logParameters,
            User = _httpContextAccessor.HttpContext == null ||
                   _httpContextAccessor.HttpContext.User.Identity.Name == null
                       ? "?"
                       : _httpContextAccessor.HttpContext.User.Identity.Name
        };

        _logger.Info(JsonConvert.SerializeObject(logDetail));

        return next();
    }
}