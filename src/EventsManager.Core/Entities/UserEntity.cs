using EventsManager.Core.Enums;

namespace EventsManager.Core.Entities
{
    public class UserEntity : BaseEntity
    {

        public string Username { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;

        public UserRole Role { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
