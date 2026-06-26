using EventsManager.Application.Common.DTOs;
using EventsManager.Application.Features.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using EventsManager.Api.Extensions;

namespace EventsManager.Api.Controllers
{
    public class AuthController : Controller
    {
        private readonly UserUseCases _userUseCases;
        public AuthController(UserUseCases userUseCases)
        {
            _userUseCases = userUseCases;
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            return await _userUseCases.Login.Execute(request)
                .ToActionResult();
        }
        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            var jti = User.FindFirst(JwtRegisteredClaimNames.Jti)!.Value;
            var expClaim = User.FindFirst("exp")!.Value;

            DateTime expiration = DateTimeOffset.FromUnixTimeSeconds(long.Parse(expClaim)).DateTime;

            return await _userUseCases.Logout.Execute(LogoutRequest.From(jti, expiration))
                .ToActionResult(HttpStatusCode.NoContent);
        }
    }
}
