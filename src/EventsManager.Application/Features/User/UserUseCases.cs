using EventsManager.Application.Features.User.Login;
using EventsManager.Application.Features.User.Logout;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventsManager.Application.Features.User
{
    public record UserUseCases(LoginHandler Login, LogoutHandler Logout);
}
