using App.Appxs.eSecurities.Usings;
using App.xContexts.Apps;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Features.Features.Auths.Roles;

public class UsersRequest
    : IRequest<UsersResponse>
{ }

public class UsersResponse
{
    public List<Res> Items { get; set; }

    public class Res
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public List<string> Roles { get; set; }
        public bool Status { get; set; }
    }
}

public class UsersHandler : IRequestHandler<UsersRequest, UsersResponse>
{
    private readonly IMapper _mapper;
    private readonly AppDbContext _context;

    public UsersHandler(IMapper mapper,
        AppDbContext context)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<UsersResponse> Handle(UsersRequest request, CancellationToken cancellationToken)
    {
        var userss = from users in _context.Users
                     join userOperationClaims in _context.UserOperationClaims
                        on users.Id equals userOperationClaims.UserId
                     join operationClaims in _context.OperationClaims
                        on userOperationClaims.OperationClaimId equals operationClaims.Id
                     group operationClaims by new { users.Id, users.FirstName, users.LastName,
                         users.Email, users.Status } into userGroup
                     select new UsersResponse.Res
                     {
                         Id = userGroup.Key.Id,
                         FirstName = userGroup.Key.FirstName,
                         LastName = userGroup.Key.LastName,
                         Email = userGroup.Key.Email,
                         Roles = userGroup.Select(r => r.Name).ToList(),
                         Status = userGroup.Key.Status
                     };

        UsersResponse res = new();

        res.Items = await userss.ToListAsync();

        return res;
    }
}