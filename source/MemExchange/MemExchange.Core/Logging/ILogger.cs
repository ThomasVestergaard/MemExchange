using System;

namespace MemExchange.Core.Logging
{
    public interface ILogger
    {
        void Info(string message);
        void Error(Exception exception, string message);
    }
}
