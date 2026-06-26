using System;
using System.Collections.Generic;
using System.Text;

namespace EventsManager.Infrastructure.Tools.Logging
{
    public interface IExceptionLogStorage
    {
        Task WriteAsync(string content);
    }
}
