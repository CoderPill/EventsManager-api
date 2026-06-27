using System;
using System.Collections.Generic;
using System.Text;

namespace EventsManager.Application.Features.User.Logout
{
    public record LogoutRequest(string Jti, DateTime ExpiresDate)
    {
        public static LogoutRequest From(string jti, DateTime expiresDate)
        {
            return new(jti, expiresDate);
        }
    }
}
