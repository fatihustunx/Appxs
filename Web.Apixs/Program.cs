using Features.Entities.Apps;
using App.Appxs.Exceptions;

namespace Web.Apixs
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the containers.

            builder.Services.AddControllers();

            builder.Services.AddAppServices
                (builder.Configuration);

            // Learn more about configuring Swagger/OpenAPI's
            // at https://aka.ms/aspnetcore/swashbuckle s.
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipelines.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            //*if (app.Environment.IsProduction())
                
                app.UseExceptionsMiddlewares();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
