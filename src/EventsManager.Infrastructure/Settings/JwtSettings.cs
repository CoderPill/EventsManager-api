using System;
using System.Collections.Generic;
using System.Text;

namespace EventsManager.Infrastructure.Settings
{
    public class JwtSettings
    {
        public string SecretKey { get; set; } = string.Empty;
        public int ExpirationInHours { get; set; }
    }
}
