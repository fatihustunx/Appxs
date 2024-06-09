using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Features.Entities.Contexts;
using Features.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Features.Features.AppEntitys;

public class AppEntityFeature2Request
: IRequest<AppEntityFeature2Response>
{
    public int id { get; set; }
}

public class AppEntityFeature2Response
{
    public int Id { get; set; }

    //@Properties
}

public class AppEntityFeature2Profile : Profile
{
    public AppEntityFeature2Profile()
    {
        CreateMap<AppEntity, AppEntityFeature2Response>().ReverseMap();
    }
}

public class AppEntityFeature2Handler : IRequestHandler<AppEntityFeature2Request, AppEntityFeature2Response>
{
    private readonly IMapper _mapper;
    private readonly AppDbContext _context;

    public AppEntityFeature2Handler(IMapper mapper, AppDbContext context)
    {
        _mapper = mapper;
        _context = context;
    }

    public async Task<AppEntityFeature2Response> Handle(AppEntityFeature2Request request, CancellationToken cancellationToken)
    {
        var entity = await _context.Set<AppEntity>().
            FirstOrDefaultAsync(x => x.Id == request.id);

        if (entity == null) { throw new Exception("Errors.*"); }

        var res = _mapper.Map<AppEntityFeature2Response>(entity);

        return res;
    }
}