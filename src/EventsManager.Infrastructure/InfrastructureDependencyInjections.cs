using EventsManager.Application.Common.Interfaces.Persistence;
using EventsManager.Application.Common.Interfaces.Tools;
using EventsManager.Core.Common.Time;
using EventsManager.Infrastructure.Persistence.Common.Context;
using EventsManager.Infrastructure.Persistence.Features.Event;
using EventsManager.Infrastructure.Persistence.Features.Reservation;
using EventsManager.Infrastructure.Persistence.Features.User;
using EventsManager.Infrastructure.Persistence.Features.Venue;
using EventsManager.Infrastructure.Settings;
using EventsManager.Infrastructure.Time;
using EventsManager.Infrastructure.Tools;
using EventsManager.Infrastructure.Tools.Email;
using EventsManager.Infrastructure.Tools.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EventsManager.Infrastructure
{
    public static class InfrastructureDependencyInjections
    {
        extension(IServiceCollection services)
        {
            public void AddInfrastructure(ConnectionStringsSettings connectionStringsSettings, SmtpSettings smtpSettings)
            {
                services.AddSingleton<IExceptionInfoExtractor, ExceptionInfoExtractor>();
                services.AddSingleton<IExceptionLogStorage, ExceptionLogStorage>();
                services.AddSingleton<IDateTimeProvider, SystemDateTimeProvider>();

                services.AddDbContext<DbContextEventsManager>(options => options.UseSqlServer(connectionStringsSettings.DefaultConnection));

                if (smtpSettings.SendEnabled)
                    services.AddScoped<IEmailService, EmailService>();
                else
                    services.AddScoped<IEmailService, NullEmailService>();

                services.AddScoped<IPasswordHasher, PasswordHasher>()
                        .AddScoped<IJwtService, JwtService>()
                        .AddScoped<IAlphaNumericCodeGenerator, AlphaNumericCodeGenerator>();

                services.AddScoped<IUserRepository, UserRepository>()
                        .AddScoped<IEventRepository, EventRepository>()
                        .AddScoped<IReservationRepository, ReservationRepository>()
                        .AddScoped<IVenueRepository, VenueRepository>();

            }
        }
    }
}
