using MemExchange.Core.SharedDto.Orders;
using ProtoBuf;

namespace MemExchange.Core.SharedDto.ClientToServer
{
    [ProtoContract]
    public class ClientToServerMessage : IClientToServerMessage
    {
        [ProtoMember(1)]
        public int ClientId { get; set; }

        [ProtoMember(2)]
        public ClientToServerMessageTypeEnum MessageType { get; set; }

        [ProtoMember(3)]
        public LimitOrderDto LimitOrder { get; set; }

        [ProtoMember(4)]
        public MarketOrderDto MarketOrder { get; set; }

        [ProtoMember(5)]
        public StopLimitOrderDto StopLimitOrder { get; set; }

        [ProtoMember(6)]
        public DuoLimitOrderDto DuoLimitOrder { get; set; }


        public ClientToServerMessage()
        {
            LimitOrder = new LimitOrderDto();
            MarketOrder = new MarketOrderDto();
            StopLimitOrder = new StopLimitOrderDto();
            DuoLimitOrder = new DuoLimitOrderDto();
            Reset();
        }

        public void Reset()
        {
            MessageType = ClientToServerMessageTypeEnum.NotSet;
            LimitOrder.Reeset();
            MarketOrder.Reset();
            StopLimitOrder.Reeset();
            DuoLimitOrder.Reset();
            ClientId = -1;
        }

        public void Update(ClientToServerMessage other)
        {
            ClientId = other.ClientId;
            MessageType = other.MessageType;
            LimitOrder.Update(other.LimitOrder);
            MarketOrder.Update(other.MarketOrder);
            StopLimitOrder.Update(other.StopLimitOrder);
            DuoLimitOrder.Update(other.DuoLimitOrder);
        }
    }
}
