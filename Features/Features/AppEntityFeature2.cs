using AutoMapper;
using Features.Entities;
using Features.Entities.Contexts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Features.Features
{
    public class AppEntityFeature2Request
        :IRequest<AppEntityFeature2Response>
    {}

    public class AppEntityFeature2Response
    {
        public List<Res> Items {  get; set; } 

        public class Res
        {
            public int Id { get; set; }
            public string Name { get; set; }
        } 
    }

    public class AppEntityFeature2Profile : Profile
    {
        public AppEntityFeature2Profile()
        {
            CreateMap<AppEntity,AppEntityFeature2Response.Res>().ReverseMap();
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
            var entites = await _context.AppEntities.ToListAsync(cancellationToken);

            var res = _mapper.Map<List<AppEntityFeature2Response.Res>>(entites);

            var res2 = new AppEntityFeature2Response { Items = res };

            return res2;
        }
    }
}