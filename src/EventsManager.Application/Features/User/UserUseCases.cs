using EventsManager.Application.Features.User.Login;
using EventsManager.Application.Features.User.Logout;

namespace EventsManager.Application.Features.User
{
    public record UserUseCases(LoginHandler Login, LogoutHandler Logout);
}
