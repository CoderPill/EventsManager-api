using EventsManager.Application.Features.User;
using EventsManager.Application.Features.User.Login;
using EventsManager.Application.Features.User.Logout;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

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
            }
        }
    }
}
