namespace EventsManager.Application.Features.User.Logout
{
    public record LogoutRequest(string Jti, DateTimeOffset ExpiresDate)
    {
        public static LogoutRequest From(string jti, DateTimeOffset expiresDate)
        {
            return new(jti, expiresDate);
        }
    }
}
