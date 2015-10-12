using Disruptor;

namespace MemExchange.Server.Incoming.Logging
{
    public interface IPerformanceRecorder : IEventHandler<ClientToServerMessageQueueItem>
    {
    }
}
