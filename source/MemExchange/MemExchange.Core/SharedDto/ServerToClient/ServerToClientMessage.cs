using MemExchange.Core.SharedDto.Orders;
using ProtoBuf;

namespace MemExchange.Core.SharedDto.ServerToClient
{
    [ProtoContract]
    public class ServerToClientMessage : IServerToClientMessage
    {
        [ProtoMember(1)]
        public ServerToClientMessageTypeEnum MessageType { get; set; }
        [ProtoMember(2)]
        public LimitOrder LimitOrder { get; set; }
        [ProtoMember(3)]
        public string Message { get; set; }

        public int ReceiverClientId { get; set; }

        public ServerToClientMessage()
        {
            LimitOrder = new LimitOrder();
            Reset();
        }

        public void Reset()
        {
            MessageType = ServerToClientMessageTypeEnum.NotSet;
            Message = string.Empty;
            LimitOrder.Reeset();
        }

        public void Update(IServerToClientMessage other)
        {
            MessageType = other.MessageType;
            LimitOrder.Update(other.LimitOrder);
            Message = other.Message;
            ReceiverClientId = other.ReceiverClientId;
        }
    }
}