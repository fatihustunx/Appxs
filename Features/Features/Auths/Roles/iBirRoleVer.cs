using System;
using MediatR;
using Features.Entities.Contexts;
using Microsoft.EntityFrameworkCore;
using App.Appxs.Exceptions;
using App.Appxs.eSecurities.Usings;
using App.Appxs.eSecurities;

namespace Features.Features.Auths.Roles;

public class BirRolVerRequest
    : IRequest<BirRolVerResponse>
    ,IAuthsRequest
{
    public string Email { get; set; }
    public string Role { get; set; }

    public string[] Roles => new[] { "Admin" };
}

public class BirRolVerResponse
{
    public bool res { get; set; }
}

public class BirRolVerHandler : IRequestHandler<BirRolVerRequest, BirRolVerResponse>
{
    private readonly AppDbContext _context;

    public BirRolVerHandler(AppDbContext context)
    {
        _context = context;

    }

    public async Task<BirRolVerResponse> Handle(BirRolVerRequest request, CancellationToken cancellationToken)
    {
        var user = await _context.Set<User>().FirstOrDefaultAsync(x => x.Email.Equals(request.Email));

        if (user == null) { throw new BusinessException("Böyle bir user yok."); }

        var rol = await _context.Set<OperationClaim>().FirstOrDefaultAsync(x => x.Name.Equals(request.Role));

        if (rol == null)
        {
            await _context.Set<OperationClaim>().AddAsync
                (new OperationClaim { Name = request.Role });

            await _context.SaveChangesAsync();
        }

        rol = await _context.Set<OperationClaim>().FirstOrDefaultAsync(x => x.Name.Equals(request.Role));

        await _context.Set<UserOperationClaim>().AddAsync(new UserOperationClaim { UserId = user.Id, OperationClaimId = rol.Id });

        await _context.SaveChangesAsync();

        var res = new BirRolVerResponse { res = true };

        return res;
    }
}