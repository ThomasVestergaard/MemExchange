using System.Collections.Generic;
using Castle.Components.DictionaryAdapter;
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
        [ProtoMember(4)]
        public List<LimitOrder> OrderList  { get; set; }


        public int ReceiverClientId { get; set; }

        public ServerToClientMessage()
        {
            LimitOrder = new LimitOrder();
            OrderList = new EditableList<LimitOrder>();
            Reset();
        }

        public void Reset()
        {
            MessageType = ServerToClientMessageTypeEnum.NotSet;
            Message = string.Empty;
            LimitOrder.Reeset();
            OrderList.Clear();
        }

        public void Update(IServerToClientMessage other)
        {
            Reset();
            MessageType = other.MessageType;
            LimitOrder.Update(other.LimitOrder);
            OrderList.AddRange(other.OrderList);
            Message = other.Message;
            ReceiverClientId = other.ReceiverClientId;
        }
    }
}