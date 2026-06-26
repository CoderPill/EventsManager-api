using EventsManager.Application.Common.DTOs;
using EventsManager.Core.Entities;

namespace EventsManager.Application.Features.User
{
    public static class UserMapper
    {
        extension(UserEntity userEntity)
        {
            public JwtGenerateRequest ToJwtGenerateRequest() => JwtGenerateRequest.From
            (
               userId: userEntity.Id,
                username: userEntity.Username,
                userRole: userEntity.Role
            );
        }
    }
}
