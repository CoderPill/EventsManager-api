using EventsManager.Application.Features.Event;
using EventsManager.Application.Features.Event.Add;
using EventsManager.Application.Features.Event.Get;
using EventsManager.Application.Features.Event.GetOccupationReport;
using EventsManager.Application.Features.Reservation;
using EventsManager.Application.Features.Reservation.Add;
using EventsManager.Application.Features.Reservation.Cancel;
using EventsManager.Application.Features.Reservation.Confirm;
using EventsManager.Application.Features.Reservation.Get;
using EventsManager.Application.Features.Reservation.GetByReservationCode;
using EventsManager.Application.Features.User;
using EventsManager.Application.Features.User.Login;
using EventsManager.Application.Features.User.Logout;
using EventsManager.Application.Features.Venue;
using EventsManager.Application.Features.Venue.Add;
using EventsManager.Application.Features.Venue.Get;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace EventsManager.Application
{
    public static class ApplicationDependencyInjections
    {
        extension(IServiceCollection services)
        {
            public void AddApplication()
            {
                services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

                services.AddScoped<UserUseCases>()
                        .AddScoped<LoginHandler>()
                        .AddScoped<LogoutHandler>();

                services.AddScoped<VenueUseCases>()
                        .AddScoped<AddVenueHandler>()
                        .AddScoped<GetVenuesHandler>();

                services.AddScoped<ReservationUseCases>()
                        .AddScoped<GetReservationsHandler>()
                        .AddScoped<GetByReservationCodeHandler>()
                        .AddScoped<AddReservationHandler>()
                        .AddScoped<CancelReservationHandler>()
                        .AddScoped<ConfirmReservationHandler>();

                services.AddScoped<EventUseCases>()
                         .AddScoped<GetEventsHandler>()
                         .AddScoped<AddEventHandler>()
                         .AddScoped<GetOccupationReportHandler>();

            }
        }
    }
}
