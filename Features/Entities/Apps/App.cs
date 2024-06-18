using App.Appxs.Apps.Logging;
using App.Appxs.Apps.Caching;
using App.Appxs.Apps.Logging.Usings;
using App.Appxs.eSecurities;
using Features.Entities.Contexts;
using Features.Features.Auths.Auths;
using Features.Features.Auths.Users;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using System.Linq;
using System;

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

            //services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase("Appxs"));

            services.AddScoped<IAuthsFeatures, AuthsFeatures>();
            services.AddScoped<IUserFeatures,UserFeatures>();

            services.AddDbContext<AppDbContext>(options => options
            .UseSqlServer(configuration.GetConnectionString("Appxs"),
                c => c.MigrationsAssembly("Web.Apixs")));

            services.AddHttpContextAccessor();

            services.AddDistributedMemoryCache();

            services.AddSingleton<ILogger, FileLogger>();

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(AuthorizationBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(CachingBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(CacheRemovingBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));

            services.AddMediatR(config =>
            {
                config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            });

            return services;
        }
    }
}