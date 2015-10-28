using System.Collections.Generic;
using Castle.Components.DictionaryAdapter;
using MemExchange.Core.SharedDto.Level1;
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
        public LimitOrderDto LimitOrder { get; set; }
        [ProtoMember(3)]
        public string Message { get; set; }
        [ProtoMember(4)]
        public List<LimitOrderDto> OrderList  { get; set; }
        [ProtoMember(5)]
        public ExecutionDto Execution { get; set; }
        [ProtoMember(6)]
        public MarketBestBidAskDto Level1 { get; set; }


        public int ReceiverClientId { get; set; }

        public ServerToClientMessage()
        {
            LimitOrder = new LimitOrderDto();
            OrderList = new EditableList<LimitOrderDto>();
            Execution = new ExecutionDto();
            Level1 = new MarketBestBidAskDto();
            Reset();
        }

        public void Reset()
        {
            ReceiverClientId = -1;
            MessageType = ServerToClientMessageTypeEnum.NotSet;
            Message = string.Empty;
            LimitOrder.Reeset();
            OrderList.Clear();
            Execution.Reset();
            Level1.Reset();
        }

        public void Update(IServerToClientMessage other)
        {
            Reset();
            MessageType = other.MessageType;
            LimitOrder.Update(other.LimitOrder);
            OrderList.AddRange(other.OrderList);
            Message = other.Message;
            ReceiverClientId = other.ReceiverClientId;
            Execution.Update(other.Execution);
            Level1.Update(other.Level1);
        }
    }
}