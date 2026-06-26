using EventsManager.Core.Constants;

namespace EventsManager.Api.Extensions
{
    public static class ApiConfigurationManagerExtensions
    {
        extension(IConfigurationManager configuration)
        {
            public T GetSetting<T>() where T : class
            {
                return configuration.GetSection(typeof(T).Name).Get<T>()
                    ?? throw new InvalidOperationException(string.Format(SystemMessages.Infrastructure.Error_InitConfiguration, typeof(T).Name));
            }
            public T GetSetting<T>(string key) where T : class
            {
                return configuration.GetSection(key).Get<T>()
                    ?? throw new InvalidOperationException(string.Format(SystemMessages.Infrastructure.Error_InitConfiguration, key));
            }
        }
    }
}
