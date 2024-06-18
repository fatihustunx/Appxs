using App.Appxs.Apps.Logging;
using App.Appxs.Apps.Caching;
using App.Appxs.eSecurities;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using App.Appxs.Apps.Logging.Usings;
using System.Reflection;
using System.Linq;
using System;
namespace App.xContexts.Apps
{
    public static class App
    {
        public static IServiceCollection AddAppServices(
            this IServiceCollection services,
            IConfiguration configuration,
                Assembly assembly)
        {   services.AddAutoMapper(assembly);
            services.AddValidatorsFromAssembly(assembly);
            services.AddMediatR(config => { config.
                RegisterServicesFromAssembly(assembly);});
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(AuthorizationBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(CacheRemovingBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(CachingBehavior<,>));
            services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase("Appxs"));
            //services.AddDbContext<AppDbContext>(options => options.UseSqlServer(configuration.
            //    GetConnectionString("Appxs"), c => c.MigrationsAssembly("App")));
            services.AddSingleton<ILogger, FileLogger>();
            services.AddDistributedMemoryCache();
            services.AddHttpContextAccessor();
            return services;}
    }
}