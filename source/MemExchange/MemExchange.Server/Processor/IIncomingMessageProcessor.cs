using Disruptor;
using MemExchange.Core.SharedDto.ClientToServer;

namespace MemExchange.Server.Processor
{
    public interface IIncomingMessageProcessor : IEventHandler<IClientToServerMessage>
    {
    }
}
