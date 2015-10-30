using System;
using Disruptor;

namespace MemExchange.Server.Incoming
{
    public class IncomingMessageQueueErrorHandler : IExceptionHandler
    {
        public void HandleEventException(Exception ex, long sequence, object evt)
        {
            Console.WriteLine("Icoming queue event exception. " + ex.Message);
        }

        public void HandleOnStartException(Exception ex)
        {
        }

        public void HandleOnShutdownException(Exception ex)
        {
        }
    }
}
