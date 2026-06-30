using EventsManager.Core.Common.Time;
using System.Diagnostics;
using System.Text;

namespace EventsManager.Infrastructure.Tools.Logging
{
    public interface IExceptionInfoExtractor
    {
        string ExtractExceptionInfo(Exception ex);
    }
    public class ExceptionInfoExtractor : IExceptionInfoExtractor
    {
        private readonly IDateTimeProvider _timeProvider;

        public ExceptionInfoExtractor(IDateTimeProvider timeProvider)
        {
            _timeProvider = timeProvider;
        }

        public string ExtractExceptionInfo(Exception ex)
        {
            var sb = new StringBuilder();
            var timestamp = _timeProvider.GetNowColombia().ToString("yyyy-MM-dd HH:mm:ss");
            sb.AppendLine($"========================================");
            sb.AppendLine($"ERROR - {timestamp}");
            sb.AppendLine($"========================================");

            sb.AppendLine($"Tipo: {ex.GetType().Name}");
            sb.AppendLine($"Mensaje: {ex.Message}");

            var stack = new StackTrace(ex, true);
            var frame = stack.GetFrame(0);

            if (frame != null)
            {
                var method = frame.GetMethod();
                var file = frame.GetFileName();
                var line = frame.GetFileLineNumber();

                sb.AppendLine($"Método: {method?.DeclaringType?.FullName}.{method?.Name}()");

                if (!string.IsNullOrEmpty(file) && line > 0)
                    sb.AppendLine($"Archivo: {Path.GetFileName(file)}:{line}");
            }

            if (ex.InnerException != null)
            {
                sb.AppendLine();
                sb.AppendLine($"--- InnerException ---");
                sb.AppendLine($"Tipo: {ex.InnerException.GetType().Name}");
                sb.AppendLine($"Mensaje: {ex.InnerException.Message}");
            }

            sb.AppendLine($"========================================");
            sb.AppendLine();

            return sb.ToString();
        }
    }
}
