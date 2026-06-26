using EventsManager.Application.Common.DTOs;
using EventsManager.Application.Common.Interfaces.Persistence;
using EventsManager.Application.Common.Interfaces.Tools;
using EventsManager.Application.Common.ResultPattern;
using EventsManager.Application.Common.UseCases;
using EventsManager.Core.Constants;
using EventsManager.Core.Entities;
using FluentValidation;

namespace EventsManager.Application.Features.User.Login
{
    public class LoginHandler : BaseUseCase<LoginRequest, string>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtService _jwtService;
        public LoginHandler(IUserRepository userRepository, IPasswordHasher passwordHasher, IJwtService jwtService, IValidator<LoginRequest> validator)
            : base(validator)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _jwtService = jwtService;
        }

        protected override async Task<Result<string>> OnExecute(LoginRequest request)
        {
            return await ValidateCredentials(request)
                        .Bind(GenerateToken);
        }
        private async Task<Result<JwtGenerateRequest>> ValidateCredentials(LoginRequest request)
        {
            UserEntity? tempUser = await _userRepository.GetByUsernameAsync(request.Username);
            if (tempUser is null || !_passwordHasher.Verify(request.Password, tempUser.PasswordHash))
            {
                return Result.Failure<JwtGenerateRequest>(SystemMessages.User.Error_Credentials);
            }
            return tempUser.ToJwtGenerateRequest();
        }
        private Result<string> GenerateToken(JwtGenerateRequest request)
        {
            return _jwtService.Generate(request);
        }
    }
}
