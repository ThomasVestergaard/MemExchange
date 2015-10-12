using System;
using MemExchange.Server.Common;

namespace MemExchange.Server.Incoming.Logging
{
    public class PerformanceRecorderTimedConsoleOutput : IPerformanceRecorder
    {
        private readonly IDateService dateService;

        public PerformanceRecorderTimedConsoleOutput(IDateService dateService)
        {
            this.dateService = dateService;
        }

        public void OnNext(ClientToServerMessageQueueItem data, long sequence, bool endOfBatch)
        {
            Console.WriteLine("Message processed. Duration: {0} milliseconds.", (dateService.UtcNow() - data.StartProcessTime).TotalMilliseconds);

        }
    }
}