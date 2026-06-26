using System;
using System.Collections.Generic;
using System.Text;

namespace EventsManager.Application.Features.User
{
    public record LoginRequest(string Username, string Password)
    {
        public static LoginRequest From(string username, string password)
        {
            return new LoginRequest(username, password);
        }
    }
    public record LogoutRequest(string Jti, DateTime ExpiresDate)
    {
        public static LogoutRequest From(string jti, DateTime expiresDate)
        {
            return new LogoutRequest(jti, expiresDate);
        }
    }
}
