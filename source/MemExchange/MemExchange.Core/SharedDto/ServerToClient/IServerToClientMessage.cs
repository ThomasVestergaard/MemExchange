using System.Collections.Generic;
using MemExchange.Core.SharedDto.Orders;

namespace MemExchange.Core.SharedDto.ServerToClient
{
    public interface IServerToClientMessage
    {
        int ReceiverClientId { get; set; }
        ServerToClientMessageTypeEnum MessageType { get; set; }
        LimitOrder LimitOrder { get; set; }
        List<LimitOrder> OrderList { get; set; }
        string Message { get; set; }
        void Update(IServerToClientMessage other);
        void Reset();
    }
}
