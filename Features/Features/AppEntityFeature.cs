using AutoMapper;
using Features.Entities;
using Features.Entities.Contexts;
using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Features.Features
{
    public class AppEntityFeatureRequest
        :IRequest<AppEntityFeatureResponse>
    {
        //@Properties
    }

    public class AppEntityFeatureResponse
    {
        public int Id { get; set; }

        //@Properties
    }

    public class AppEntityFeatureProfile : Profile
    {
        public AppEntityFeatureProfile()
        {
            CreateMap<AppEntityFeatureRequest, AppEntity>().ReverseMap();
            CreateMap<AppEntity, AppEntityFeatureResponse>().ReverseMap();
        }
    }

    public class AppEntityFeatureValidator : AbstractValidator<AppEntity>
    {
        public AppEntityFeatureValidator()
        {
            //RuleFor(x => x.Id).GreaterThan(3);
            //RuleFor(x => x.Name).NotNull().NotEmpty();

            // Rules.
        }
    }

    public class AppEntityFeatureHandler : IRequestHandler<AppEntityFeatureRequest, AppEntityFeatureResponse>
    {
        private readonly IMapper _mapper;
        private readonly AppDbContext _context;
        private readonly IValidator<AppEntity> _validator;

        public AppEntityFeatureHandler(IMapper mapper, AppDbContext context,
            IValidator<AppEntity> validator)
        {
            _mapper = mapper;
            _context = context;
            _validator = validator;
        }

        public async Task<AppEntityFeatureResponse> Handle(AppEntityFeatureRequest request, CancellationToken cancellationToken)
        {
            var entity = _mapper.Map<AppEntity>(request);

            var valRes = await _validator.ValidateAsync(entity);

            if(!valRes.IsValid)
            {
                throw new Exception("Erros.");
            }

            await _context.AddAsync(entity);

            await _context.SaveChangesAsync();

            var res = _mapper.Map<AppEntityFeatureResponse>(entity);

            return res;
        }
    }
}