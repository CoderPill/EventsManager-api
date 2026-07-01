using EventsManager.Api.Extensions;
using EventsManager.Application.Common.DTOs;
using EventsManager.Application.Common.ResultPattern;
using EventsManager.Application.Features.User;
using EventsManager.Application.Features.User.Login;
using EventsManager.Application.Features.User.Logout;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace EventsManager.Api.Controllers
{
    /// <summary>
    /// Endpoints de autenticación: login y logout.
    /// </summary>
    /// <remarks>
    /// El login es público (no requiere token). El logout requiere JWT válido.
    /// Todas las respuestas usan el envoltorio Result&lt;T&gt;: { isSuccess, value, errors }.
    /// </remarks>
    public class AuthController : BaseApiController
    {
        private readonly UserUseCases _userUseCases;
        public AuthController(UserUseCases userUseCases)
        {
            _userUseCases = userUseCases;
        }
        /// <summary>
        /// Autentica un usuario y devuelve un token JWT.
        /// </summary>
        /// <param name="request">
        /// Credenciales de acceso:
        /// <list type="bullet">
        /// <item><strong>Username</strong> (string, requerido): nombre de usuario registrado.</item>
        /// <item><strong>Password</strong> (string, requerido): contraseña.</item>
        /// </list>
        /// </param>
        /// <response code="200">Token JWT en Result&lt;string&gt;.value.</response>
        /// <response code="400">Credenciales inválidas (Credenciales Invalidas).</response>
        [AllowAnonymous]
        [HttpPost("login")]
        [ProducesResponseType(typeof(Result<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result<Unit>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            return await _userUseCases.Login.Execute(request)
                .ToActionResult();
        }
        /// <summary>
        /// Revoca el token JWT actual (logout).
        /// </summary>
        /// <remarks>
        /// Requiere header Authorization: Bearer &lt;token&gt;.
        /// Extrae jti y exp del token para agregarlo a la lista de revocados.
        /// </remarks>
        /// <response code="200">Logout exitoso.</response>
        /// <response code="401">Token inválido, expirado o revocado.</response>
        [HttpPost("logout")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result<Unit>), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Logout()
        {
            var jti = User.FindFirst(JwtRegisteredClaimNames.Jti)!.Value;
            var expClaim = User.FindFirst("exp")!.Value;

            DateTimeOffset expiration = DateTimeOffset.FromUnixTimeSeconds(long.Parse(expClaim));

            return await _userUseCases.Logout.Execute(LogoutRequest.From(jti, expiration))
                .ToActionResult();
        }
    }
}