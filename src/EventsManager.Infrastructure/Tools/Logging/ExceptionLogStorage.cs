namespace EventsManager.Infrastructure.Tools.Logging
{
    public class ExceptionLogStorage : IExceptionLogStorage
    {
        private readonly SemaphoreSlim _semaphore = new(1, 1);
        private readonly string _logDirectory;

        public ExceptionLogStorage()
        {
            _logDirectory = Path.Combine(Directory.GetCurrentDirectory(), "ExceptionLogs");
            EnsureDirectoryExists();
        }

        public async Task WriteAsync(string content)
        {
            await _semaphore.WaitAsync();
            try
            {
                var fileName = $"Errors-{DateTime.Now:yyyy-MM-dd}.txt";
                var filePath = Path.Combine(_logDirectory, fileName);
                await File.AppendAllTextAsync(filePath, content);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private void EnsureDirectoryExists()
        {
            if (!Directory.Exists(_logDirectory))
                Directory.CreateDirectory(_logDirectory);
        }
    }
}
