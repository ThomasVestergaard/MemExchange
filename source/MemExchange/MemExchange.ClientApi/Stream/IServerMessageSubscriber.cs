using System;
using MemExchange.Core.SharedDto.ServerToClient;

namespace MemExchange.ClientApi.Stream
{
    public interface IServerMessageSubscriber
    {
        void Start(string serverAddress, int serverPublishPort, int clientId, Action<ServerToClientMessage> messageHandler);
        void Stop();
    }
}
