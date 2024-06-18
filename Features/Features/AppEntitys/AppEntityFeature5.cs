﻿using AutoMapper;
using Features.Entities;
//@LogUsing
//@CacheUsing
//@AuthsUsing
using Features.Entities.Contexts;
using Microsoft.EntityFrameworkCore;
using MediatR;
using System;

namespace Features.Features.AppEntitys;

public class AppEntityFeature5Request
: IRequest<AppEntityFeature5Response>
    //@LogReq//@CacherReq//@AuthsReq
{
    //@AuthsParams
    //@CacherParams
    public int id { get; set; }
}

public class AppEntityFeature5Response
{
    public bool res { get; set; }
}

public class AppEntityFeature5Profile : Profile
{
    public AppEntityFeature5Profile()
    {
        CreateMap<AppEntity, AppEntityFeature5Request>().ReverseMap();
    }
}

public class AppEntityFeature5Handler : IRequestHandler<AppEntityFeature5Request, AppEntityFeature5Response>
{
    private readonly IMapper _mapper;
    private readonly AppDbContext _context;

    public AppEntityFeature5Handler(IMapper mapper, AppDbContext context)
    {
        _mapper = mapper;
        _context = context;
    }

    public async Task<AppEntityFeature5Response> Handle(AppEntityFeature5Request request, CancellationToken cancellationToken)
    {
        AppEntityFeature5Response res = new();

        var isExists = await _context.Set<AppEntity>().AnyAsync(c => c.Id == request.id);

        if (!isExists) { res.res = false; return res; } //Errors.***.

        var entity = _mapper.Map<AppEntity>(request);

        _context.Set<AppEntity>().Remove(entity);
        await _context.SaveChangesAsync();

        res.res = true;
        return res;
    }
}
