using EventsManager.Application.Common.Interfaces.Tools;
using EventsManager.Application.Common.ResultPattern;
using EventsManager.Core.Constants;
using EventsManager.Infrastructure.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Text;

namespace EventsManager.Api.Extensions
{
    public static class ApiServiceCollectionExtensions
    {
        extension(IServiceCollection services)
        {
            public IServiceCollection CustomAuthentication(JwtSettings jwtSettings)
            {
                services.AddMemoryCache();
                services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                 .AddJwtBearer(options =>
                 {
                     options.SaveToken = false;
                     options.TokenValidationParameters = new TokenValidationParameters
                     {
                         ValidateIssuer = false,
                         ValidateAudience = false,
                         ValidateLifetime = true,
                         ClockSkew = TimeSpan.Zero,
                         ValidateIssuerSigningKey = true,
                         IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey))
                     };

                     options.Events = new JwtBearerEvents
                     {
                         OnTokenValidated = context =>
                         {
                             var revocationService = context.HttpContext.RequestServices.GetRequiredService<IJwtService>();

                             var jti = context.Principal?.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;

                             if (string.IsNullOrEmpty(jti))
                                 context.Fail(SystemMessages.User.Error_InvalidToken);
                             else if (revocationService.IsRevoked(jti))
                                 context.Fail(SystemMessages.User.Error_RevokedToken);

                             return Task.CompletedTask;
                         },
                         OnChallenge = async context =>
                         {
                             context.HandleResponse();

                             context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                             context.Response.ContentType = SystemValues.Infrastructure.JsonContentType;

                             var result = Result.Failure(context.ErrorDescription ?? SystemMessages.User.Error_Unauthorized);

                             await context.Response.WriteAsJsonAsync(result);
                         },

                         OnAuthenticationFailed = async context =>
                         {
                             context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                             context.Response.ContentType = SystemValues.Infrastructure.JsonContentType;

                             var result = Result.Failure(SystemMessages.User.Error_InvalidToken);

                             await context.Response.WriteAsJsonAsync(result);
                         }
                     };
                 });

                services.AddAuthorization();
                return services;
            }

            public void SwaggerConfiguration()
            {
                services.AddEndpointsApiExplorer();
                services.AddSwaggerGen(opt =>
                {
                    // Información básica
                    opt.SwaggerDoc("v1", new OpenApiInfo
                    {
                        Title = "EventsManager API",
                        Version = "v1",
                        Description = "API para la gestión de eventos"
                    });
                    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                    if (File.Exists(xmlPath))
                    {
                        opt.IncludeXmlComments(xmlPath);
                    }
                    // Definición del esquema
                    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                    {
                        // Usar Http‑Bearer es la forma que Swashbuckle reconoce como “Authorize”
                        Type = SecuritySchemeType.Http,
                        In = ParameterLocation.Header,
                        Name = "Authorization",
                        Scheme = "Bearer",
                        Description = "Introduce el token JWT'"
                    });

                    // Requerir el esquema en todas las operaciones
                    opt.AddSecurityRequirement(req => new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecuritySchemeReference("Bearer", req),
                            new List<string>()

                        }
                    });
                });
            }

            public void CORSConfiguration(string[] allowedOrigins)
            {
                services.AddCors(options =>
                {
                    options.AddPolicy(SystemValues.Infrastructure.CORSPolicyName, builder =>
                    {
                        builder.WithOrigins(allowedOrigins)
                               .AllowAnyMethod()
                               .AllowAnyHeader();
                    });
                });
            }
        }
    }
}
