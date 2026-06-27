using EventsManager.Core.Enums;

namespace EventsManager.Application.Common.DTOs
{
    public record JwtRevokeRequest(string Jti, DateTime ExpiresDate)
    {
        public static JwtRevokeRequest From(string jti, DateTime expiresDate)
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
