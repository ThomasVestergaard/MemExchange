using MemExchange.Core.SharedDto.Orders;

namespace MemExchange.Core.SharedDto.ServerToClient
{
    public interface IServerToClientMessage
    {
        int ReceiverClientId { get; set; }
        ServerToClientMessageTypeEnum MessageType { get; set; }
        LimitOrder LimitOrder { get; set; }
        void Update(IServerToClientMessage other);
        void Reset();
    }
}
