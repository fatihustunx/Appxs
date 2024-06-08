using AutoMapper;
using Features.Entities.Contexts;
using Features.Entities;
using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Features.Features
{
    public class AppEntityFeature4
    {
        public class AppEntityFeature4Request
        : IRequest<AppEntityFeature4Response>
        {
            public int Id { get; set; }

            //@Properties
        }

        public class AppEntityFeature4Response
        {
            public int Id { get; set; }

            //@Properties
        }

        public class AppEntityFeature4Profile : Profile
        {
            public AppEntityFeature4Profile()
            {
                CreateMap<AppEntityFeature4Request, AppEntity>().ReverseMap();
                CreateMap<AppEntity, AppEntityFeature4Response>().ReverseMap();
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
                    throw new Exception("Erros.");
                }

                var isExists = await _context.Set<AppEntity>()
                    .AnyAsync(c => c.Id == request.Id);

                if (!isExists)
                {
                    throw new Exception("Errors.**");
                }

                _context.Update(entity);

                await _context.SaveChangesAsync();

                var res = _mapper.Map<AppEntityFeature4Response>(entity);

                return res;
            }
        }
    }
}
