using System;
using Serilog;

namespace MemExchange.Core.Logging
{
    public class SerilogLogger : ILogger
    {
        private Serilog.ILogger logger;

        public SerilogLogger()
        {
            logger = new LoggerConfiguration()
                .WriteTo.RollingFile("log.txt")
                .WriteTo.ColoredConsole()
                .CreateLogger();
        }

        public void Info(string message)
        {
            logger.Information(message);
        }

        public void Error(Exception exception, string message)
        {
            logger.Error(exception, message);
        }
    }
}
