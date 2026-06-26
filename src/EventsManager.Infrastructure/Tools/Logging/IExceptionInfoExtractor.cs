namespace EventsManager.Infrastructure.Tools.Logging
{
    public interface IExceptionInfoExtractor
    {
        string ExtractExceptionInfo(Exception ex);
    }
}
