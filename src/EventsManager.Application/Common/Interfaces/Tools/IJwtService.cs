using EventsManager.Application.Common.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventsManager.Application.Common.Interfaces.Tools
{
    public interface IJwtService
    {
        string Generate(JwtGenerateRequest request);
        void Revoke(JwtRevokeRequest request);
        bool IsRevoked(string jti);
    }
}
