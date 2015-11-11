using Disruptor;

namespace MemExchange.Server.Incoming.Logging
{
    public interface IPerformanceRecorder : IEventHandler<RingbufferByteArray>
    {
        void Setup(RingBuffer<RingbufferByteArray> ringBuffer, int outputMetricsPerCount);
    }
}
