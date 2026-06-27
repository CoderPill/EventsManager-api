using EventsManager.Application.Common.DTOs;
using EventsManager.Core.Entities;

namespace EventsManager.Application.Features.User
{
    public static class UserMapper
    {
        extension(UserEntity instance)
        {
            public JwtGenerateRequest ToJwtGenerateRequest()
            {
                return JwtGenerateRequest.From(instance.Id,instance.Username,instance.Role );
            }
        }
    }
}
