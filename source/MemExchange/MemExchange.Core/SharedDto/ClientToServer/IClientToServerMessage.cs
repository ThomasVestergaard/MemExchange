using MemExchange.Core.SharedDto.Orders;

namespace MemExchange.Core.SharedDto.ClientToServer
{
    public interface IClientToServerMessage
    {
        int ClientId { get; set; }
        LimitOrderDto LimitOrder { get; set; }
        MarketOrderDto MarketOrder { get; set; }
        ClientToServerMessageTypeEnum MessageType { get; set; }
        void Reset();
        void Update(ClientToServerMessage other);
    }
}