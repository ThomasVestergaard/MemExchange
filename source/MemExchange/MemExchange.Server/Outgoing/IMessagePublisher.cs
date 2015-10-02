using Disruptor;
using MemExchange.Core.SharedDto.ServerToClient;

namespace MemExchange.Server.Outgoing
{
    public interface IMessagePublisher : IEventHandler<ServerToClientMessage>
    {
        void Start(int publishPort);
        void Stop();
        void Publish(int clientId, ServerToClientMessage serverToClientMessage);
    }
}
