
using EventsManager.Api.Extensions;
using EventsManager.Api.Middlewares;
using EventsManager.Application;
using EventsManager.Core.Constants;
using EventsManager.Infrastructure;
using EventsManager.Infrastructure.Settings;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EventsManager.Api
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            //Carga las variables de entorno que concuerden con el prefijo definido en SystemValues.EnvironmentPrefix
            builder.Configuration.AddEnvironmentVariables(prefix: SystemValues.Infrastructure.EnvironmentPrefix);
            //Obtiene los valores de configuracion
            string[] allowedOrigins = builder.Configuration.GetSetting<string[]>(SystemValues.Infrastructure.CORSPolicyName);
            ConnectionStringsSettings connectionStringsSettings = builder.Configuration.GetSetting<ConnectionStringsSettings>();
            AdminSeedSettings adminSeedSettings = builder.Configuration.GetSetting<AdminSeedSettings>();
            JwtSettings jwtSettings = builder.Configuration.GetSetting<JwtSettings>();

            builder.Services.AddSingleton(jwtSettings);

            builder.Services.AddControllers().AddJsonOptions((options) =>
            {
                options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });
            builder.Services.CORSConfiguration(allowedOrigins);
            builder.Services.CustomAuthentication(jwtSettings);
            builder.Services.SwaggerConfiguration();

            builder.Services.AddInfrastructure(connectionStringsSettings);
            builder.Services.AddApplication();

            var app = builder.Build();
            app.UseMiddleware<GlobalExceptionMiddleware>();
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger().UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Events Manager API");
                });
            }
            app.UseCors(SystemValues.Infrastructure.CORSPolicyName);
            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();
            app.MapGet("/", () => "API Running.");
            //migracion automatica y establecer valores por defecto en base de datos
            await app.ApplyMigrationsAndSeedAsync(adminSeedSettings);

            app.Run();
        }
    }
}
