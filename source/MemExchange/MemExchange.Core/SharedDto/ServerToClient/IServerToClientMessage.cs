using System.Collections.Generic;
using MemExchange.Core.SharedDto.Level1;
using MemExchange.Core.SharedDto.Orders;

namespace MemExchange.Core.SharedDto.ServerToClient
{
    public interface IServerToClientMessage
    {
        int ReceiverClientId { get; set; }
        ServerToClientMessageTypeEnum MessageType { get; set; }
        LimitOrderDto LimitOrder { get; set; }
        List<LimitOrderDto> LimitOrderList { get; set; }
        List<StopLimitOrderDto> StopLimitOrderList { get; set; }
        string Message { get; set; }
        void Update(IServerToClientMessage other);
        void Reset();
        ExecutionDto Execution { get; set; }
        MarketBestBidAskDto Level1 { get; set; }
        StopLimitOrderDto StopLimitOrder { get; set; }
    }
}
