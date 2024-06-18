using App.Appxs.eSecurities.Usings;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Http;
using App.Appxs.Exceptions;
using MediatR;
using System;

namespace App.Appxs.eSecurities;

public interface IAuthsRequest
{
    public string[] Roles { get; }
}

public class AuthorizationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>, IAuthsRequest
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthorizationBehavior(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<TResponse> Handle(TRequest request,RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        List<string>? roleClaims = _httpContextAccessor.HttpContext.User.ClaimRoles();

        if (roleClaims == null) throw new AuthorizationException("User rolleri yok.");

        bool isNotMatchedARoleClaimWithRequestRoles =
            roleClaims.FirstOrDefault(roleClaim => request.Roles.Any(role => role == roleClaim)).IsNullOrEmpty();
        if (isNotMatchedARoleClaimWithRequestRoles) throw new AuthorizationException("Bu işlem için yetkiniz yok.");

        TResponse response = await next();

        return response;
    }
}