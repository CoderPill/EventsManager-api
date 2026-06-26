using EventsManager.Application.Common.DTOs;
using EventsManager.Application.Common.Interfaces.Tools;
using EventsManager.Infrastructure.Settings;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static EventsManager.Core.Constants.SystemMessages;

namespace EventsManager.Infrastructure.Tools
{
    public class JwtService : IJwtService
    {
        private readonly JwtSettings _jwtSettings;
        private readonly IMemoryCache _cache;
        public JwtService(IMemoryCache cache, JwtSettings jwtSettings)
        {
            _cache = cache;
            _jwtSettings = jwtSettings;
        }
        public string Generate(JwtGenerateRequest request)
        {
            var claims = new[]
           {
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.NameIdentifier, request.UserId.ToString()),
            new Claim(ClaimTypes.Name, request.Username),
            new Claim(ClaimTypes.Role, request.UserRole.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

            var jwtConfiguration = new JwtSecurityToken(
                                        claims: claims,
                                        expires: DateTime.Now.AddHours(_jwtSettings.ExpirationInHours),
                                        signingCredentials: credentials
                                        );
            return new JwtSecurityTokenHandler().WriteToken(jwtConfiguration);
        }

        public bool IsRevoked(string jti)
        {
            return _cache.TryGetValue(jti, out _);
        }

        public void Revoke(JwtRevokeRequest request)
        {
            var cacheOptions = new MemoryCacheEntryOptions()
            {
                AbsoluteExpiration = request.ExpiresDate
            };
            _cache.Set(request.Jti, true, cacheOptions);
        }
    }
}
