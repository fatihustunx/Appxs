using AutoMapper;
using Features.Entities;//@LogUsing//@CacheUsing//@AuthsUsing
using Features.Entities.Contexts;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using App.Appxs.Exceptions;
using MediatR;
using System;

namespace Features.Features.AppEntitys;

public class AppEntityFeature4Request
    : IRequest<AppEntityFeature4Response>
    //@LogReq//@CacherReq//@AuthsReq
{//@AuthsParams//@CacherParams//@Ids//@Properties
 }

public class AppEntityFeature4Response
{
    public int Id { get; set; }//@Properties
}

public class AppEntityFeature4Profile : Profile
{
    public AppEntityFeature4Profile()
    {
        CreateMap<AppEntityFeature4Request, AppEntity>().ReverseMap();
        CreateMap<AppEntity, AppEntityFeature4Response>().ReverseMap();
    }
}

public class AppEntityFeature4Validator : AbstractValidator<AppEntity>
{
    public AppEntityFeature4Validator()
    {
        // Gets Creator Rules.
    }
}

public class AppEntityFeature4Handler : IRequestHandler<AppEntityFeature4Request, AppEntityFeature4Response>
{
    private readonly IMapper _mapper;
    private readonly AppDbContext _context;
    private readonly IValidator<AppEntity> _validator;

    public AppEntityFeature4Handler(IMapper mapper, AppDbContext context,
        IValidator<AppEntity> validator)
    {
        _mapper = mapper;
        _context = context;
        _validator = validator;
    }

    public async Task<AppEntityFeature4Response> Handle(AppEntityFeature4Request request, CancellationToken cancellationToken)
    {
        var entity = _mapper.Map<AppEntity>(request);

        var valRes = await _validator.ValidateAsync(entity);

        if (!valRes.IsValid)
        {
            throw new ValidationException(valRes.Errors);
        }

        var isExists = await _context.Set<AppEntity>()
            .AnyAsync(c => c.Id == request.Id);

        if (!isExists)
        {
            throw new BusinessException("Böyle bir varlik yok.");
        }

        _context.Update(entity);

        await _context.SaveChangesAsync();

        var res = _mapper.Map<AppEntityFeature4Response>(entity);

        return res;
    }
}