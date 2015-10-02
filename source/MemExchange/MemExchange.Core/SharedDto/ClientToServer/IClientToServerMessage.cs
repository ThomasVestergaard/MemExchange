using MemExchange.Core.SharedDto.Orders;

namespace MemExchange.Core.SharedDto.ClientToServer
{
    public interface IClientToServerMessage
    {
        int ClientId { get; set; }
        LimitOrder LimitOrder { get; set; }
        ClientToServerMessageTypeEnum MessageType { get; set; }
        void Reset();
        void Update(ClientToServerMessage other);
    }
}