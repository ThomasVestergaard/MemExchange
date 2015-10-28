using Disruptor;

namespace MemExchange.Server.Incoming.Logging
{
    public interface IPerformanceRecorder : IEventHandler<ClientToServerMessageQueueItem>
    {
        void Setup(RingBuffer<ClientToServerMessageQueueItem> ringBuffer, int outputMetricsPerCount);
    }
}
