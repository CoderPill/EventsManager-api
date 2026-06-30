using EventsManager.Core.Enums;

namespace EventsManager.Application.Common.DTOs
{
    public record JwtRevokeRequest(string Jti, DateTimeOffset ExpiresDate)
    {
        public static JwtRevokeRequest From(string jti, DateTimeOffset expiresDate)
        {
            return new(jti, expiresDate);
        }
    }
    public record JwtGenerateRequest(int UserId, string Username, UserRole UserRole)
    {
        public static JwtGenerateRequest From(int userId, string username, UserRole userRole)
        {
            return new(userId, username, userRole);
        }
    }

}
