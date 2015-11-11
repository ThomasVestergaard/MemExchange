using Disruptor;
using MemExchange.Server.Incoming;

namespace MemExchange.Server.Processor
{
    public interface IIncomingMessageProcessor : IEventHandler<RingbufferByteArray>
    {
    }
}
