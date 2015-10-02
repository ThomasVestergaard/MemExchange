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
        public LimitOrder LimitOrder { get; set; }

        public ClientToServerMessage()
        {
            LimitOrder = new LimitOrder();
            Reset();
        }

        public void Reset()
        {
            MessageType = ClientToServerMessageTypeEnum.NotSet;
            this.LimitOrder.Reeset();
            ClientId = -1;
        }

        public void Update(ClientToServerMessage other)
        {
            ClientId = other.ClientId;
            MessageType = other.MessageType;
            LimitOrder = other.LimitOrder;

        }
    }
}
