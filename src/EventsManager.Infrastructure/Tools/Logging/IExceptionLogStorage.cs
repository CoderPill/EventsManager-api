namespace EventsManager.Infrastructure.Tools.Logging
{
    public interface IExceptionLogStorage
    {
        Task WriteAsync(string content);
    }
}
