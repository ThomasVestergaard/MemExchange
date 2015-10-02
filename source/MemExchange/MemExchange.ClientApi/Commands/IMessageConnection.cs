using MemExchange.Core.SharedDto.ClientToServer;

namespace MemExchange.ClientApi.Commands
{
    public interface IMessageConnection
    {
        void Start(string serverIpAddress, int serverPort);
        void Stop();
        void SendMessage(ClientToServerMessage message);

    }
}
