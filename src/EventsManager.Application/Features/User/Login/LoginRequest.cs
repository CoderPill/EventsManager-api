namespace EventsManager.Application.Features.User.Login
{
    public record LoginRequest(string Username, string Password)
    {
        public static LoginRequest From(string username, string password)
        {
            return new(username, password);
        }
    }
}
