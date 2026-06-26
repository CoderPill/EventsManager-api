using EventsManager.Application.Common.DTOs;
using EventsManager.Application.Common.Interfaces.Tools;
using EventsManager.Application.Common.ResultPattern;
using EventsManager.Application.Common.UseCases;
using FluentValidation;

namespace EventsManager.Application.Features.User.Logout
{
    public class LogoutHandler : BaseUseCase<LogoutRequest, Unit>
    {
        private readonly IJwtService _jwtService;
        public LogoutHandler(IJwtService jwtService, IValidator<LogoutRequest> validator)
            : base(validator)
        {
            _jwtService = jwtService;
        }

        protected override Task<Result<Unit>> OnExecute(LogoutRequest request)
        {
            _jwtService.Revoke(JwtRevokeRequest.From(request.Jti, request.ExpiresDate));
            return Result.Success().ToTask();
        }
    }
}
