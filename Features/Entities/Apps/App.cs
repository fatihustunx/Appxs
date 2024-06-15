using Features.Entities.Contexts;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Features.Entities.Apps
{
    public static class App
    {
        public static IServiceCollection AddAppServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase("Appxs"));

            //services.AddDbContext<AppDbContext>(options => options
            //.UseSqlServer(configuration.GetConnectionString("Appxs"),
            //    c => c.MigrationsAssembly("Web.Apixs")));

            services.AddMediatR(config =>
            {
                config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            });

            return services;
        }
    }
}