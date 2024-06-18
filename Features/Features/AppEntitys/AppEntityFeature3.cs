using AutoMapper;
using Features.Entities;
using Features.Entities.Contexts;
using Microsoft.EntityFrameworkCore;
//@LogUsing
//@CacheUsing
//@AuthsUsing
using MediatR;
using System;

namespace Features.Features.AppEntitys;

public class AppEntityFeature3Request
    : IRequest<AppEntityFeature3Response>
    //@LogReq//@CacheReq//@AuthsReq
{
    //@AuthsParams
    //@CacheParams
}

public class AppEntityFeature3Response
{
    public List<Res> Items { get; set; }

    public class Res
    {
        public int Id { get; set; }
        //@Properties
    }
}

public class AppEntityFeature3Profile : Profile
{
    public AppEntityFeature3Profile()
    {
        CreateMap<AppEntity, AppEntityFeature3Response.Res>().ReverseMap();
    }
}

public class AppEntityFeature3Handler : IRequestHandler<AppEntityFeature3Request, AppEntityFeature3Response>
{
    private readonly IMapper _mapper;
    private readonly AppDbContext _context;

    public AppEntityFeature3Handler(IMapper mapper, AppDbContext context)
    {
        _mapper = mapper;
        _context = context;
    }

    public async Task<AppEntityFeature3Response> Handle(AppEntityFeature3Request request, CancellationToken cancellationToken)
    {
        var entites = await _context.AppEntities.ToListAsync(cancellationToken);

        var res = _mapper.Map<List<AppEntityFeature3Response.Res>>(entites);

        var res3 = new AppEntityFeature3Response { Items = res };

        return res3;
    }
}