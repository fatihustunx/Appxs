using AutoMapper;
using App.xContexts.Apps;
using Microsoft.EntityFrameworkCore;
using App.Appxs.eSecurities.Usings;
using MediatR;
using System;

namespace Features.Features.Auths.Roles;

public class RolesRequest
    : IRequest<RolesResponse>
{}

public class RolesResponse
{
    public List<Res> Items { get; set; }

    public class Res
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}

public class RolesProfile : Profile
{
    public RolesProfile()
    {
        CreateMap<OperationClaim, RolesResponse.Res>().ReverseMap();
    }
}

public class RolesHandler : IRequestHandler<RolesRequest, RolesResponse>
{
    private readonly IMapper _mapper;
    private readonly AppDbContext _context;

    public RolesHandler(IMapper mapper,
        AppDbContext context)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<RolesResponse> Handle(RolesRequest request, CancellationToken cancellationToken)
    {
        var entities = await _context.OperationClaims.ToListAsync(cancellationToken);

        var res = _mapper.Map<List<RolesResponse.Res>>(entities);

        var res3 = new RolesResponse { Items = res };

        return res3;
    }
}