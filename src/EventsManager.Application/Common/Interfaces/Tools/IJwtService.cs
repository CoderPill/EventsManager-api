using EventsManager.Application.Common.DTOs;

namespace EventsManager.Application.Common.Interfaces.Tools
{
    public interface IJwtService
    {
        string Generate(JwtGenerateRequest request);
        void Revoke(JwtRevokeRequest request);
        bool IsRevoked(string jti);
    }
}
